using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Users.Commands.GenerateRefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
    {
        private readonly IAppDbContext _context;
        private readonly IJwtTokenGenerator _jwt;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RefreshTokenHandler(IAppDbContext context, IJwtTokenGenerator jwt, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _jwt = jwt;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {

            // Hash du token reçu pour comparer avec la DB
            var tokenHash = RefreshToken.Hash(request.Token);

            // Lookup — cherche directement le token par son hash
            var existingToken = await _context.RefreshTokens
                .Include(t => t.User)
                    .ThenInclude(u => u.RefreshTokens)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken)
                ?? throw new UnauthorizedAccessException("Invalid refresh token");

            var user = existingToken.User;

            // Token révoqué — détection de réutilisation (rotation attack)
            if (existingToken.IsRevoked)
            {
                // Révoque TOUS les tokens du user par sécurité
                foreach (var token in user.RefreshTokens.Where(t => t.IsActive))
                    token.Revoke(_dateTimeProvider.UtcNow);

                await _context.SaveChangesAsync(cancellationToken);
                throw new UnauthorizedAccessException(
                    "Token reuse detected. All sessions have been revoked.");
            }

            if (existingToken.IsExpired)
                throw new UnauthorizedAccessException("Refresh token has expired");

            // Génère de nouveaux tokens
            var tokens = _jwt.GenerateTokens(user.Id, user.Email.Value);

            // Révoque l'ancien avec référence au nouveau
            existingToken.Revoke(_dateTimeProvider.UtcNow, tokens.RawRefreshToken);

            // Limite de sessions actives à 5
            var activeTokens = user.RefreshTokens
                .Where(t => t.IsActive)
                .OrderBy(t => t.CreatedAt)
                .ToList();

            if (activeTokens.Count >= 5)
            {
                var oldest = activeTokens.First();
                oldest.Revoke(_dateTimeProvider.UtcNow);
            }

            // Attache le nouveau token
            user.AddRefreshToken(tokens.RefreshToken);

            // Nettoie les tokens inactifs
            var toRemove = user.RefreshTokens
                .Where(t => !t.IsActive && t.RevokedAt < _dateTimeProvider.UtcNow.AddDays(-30))
                .ToList();

            foreach (var old in toRemove)
                user.CleanupRefreshTokens(_dateTimeProvider.UtcNow);

            await _context.SaveChangesAsync(cancellationToken);

            return new RefreshTokenResult(tokens.AccessToken, tokens.RawRefreshToken);
        }
    }
}
