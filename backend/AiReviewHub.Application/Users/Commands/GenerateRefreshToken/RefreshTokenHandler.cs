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
            // Cherche le user via son refresh token
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u =>
                    u.RefreshTokens.Any(t => t.Token == request.Token),
                    cancellationToken)
                ?? throw new UnauthorizedAccessException("Invalid refresh token");

            var existingToken = user.RefreshTokens
                .First(t => t.Token == request.Token);

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

            if (!existingToken.IsActive)
                throw new UnauthorizedAccessException("Refresh token has expired");

            var now = _dateTimeProvider.UtcNow;

            // Génère de nouveaux tokens
            var tokens = _jwt.GenerateTokens(user.Id, user.Email.Value);

            // Révoque l'ancien et crée le nouveau (rotation)
            existingToken.Revoke(now, tokens.RefreshToken);
            var newRefreshToken = RefreshToken.Create(user.Id, now);
            user.RefreshTokens.Add(newRefreshToken);

            // Nettoie les tokens inactifs
            var toRemove = user.RefreshTokens
                .Where(t => !t.IsActive && t.Id != newRefreshToken.Id)
                .ToList();

            foreach (var old in toRemove)
                user.RefreshTokens.Remove(old);

            await _context.SaveChangesAsync(cancellationToken);

            return new RefreshTokenResult(tokens.AccessToken, newRefreshToken.Token);
        }
    }
}
