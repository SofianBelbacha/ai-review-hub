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
        private readonly ITokenService _tokenService;


        public RefreshTokenHandler(IAppDbContext context, IJwtTokenGenerator jwt, IDateTimeProvider dateTimeProvider, ITokenService tokenService)
        {
            _context = context;
            _jwt = jwt;
            _dateTimeProvider = dateTimeProvider;
            _tokenService = tokenService;
        }

        public async Task<RefreshTokenResult> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            var tokenHash = RefreshToken.Hash(request.Token);

            // Cherche le token existant avec son user et ses tokens
            var existingToken = await _context.RefreshTokens
                .Include(t => t.User)
                    .ThenInclude(u => u.RefreshTokens)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken)
                ?? throw new UnauthorizedAccessException("Invalid refresh token");

            var user = existingToken.User;
            var now = _dateTimeProvider.UtcNow;

            // Détection de réutilisation
            if (existingToken.IsRevoked)
            {
                foreach (var t in user.RefreshTokens.Where(t => t.IsActive))
                    t.Revoke(now);

                await _context.SaveChangesAsync(cancellationToken);
                throw new UnauthorizedAccessException(
                    "Token reuse detected. All sessions have been revoked.");
            }

            if (existingToken.IsExpired)
                throw new UnauthorizedAccessException("Refresh token has expired");

            var session = _tokenService.PrepareSession(user, now);

            // Handler orchestre tout
            existingToken.Revoke(now, session.RawRefreshToken);

            _context.RefreshTokens.Add(session.RefreshTokenEntity);
            await _context.SaveChangesAsync(cancellationToken);

            // Nettoyage séparé
            await _context.RefreshTokens
                .Where(t =>
                    t.UserId == user.Id &&
                    t.RevokedAt != null &&
                    t.RevokedAt < now.AddDays(-30))
                .ExecuteDeleteAsync(cancellationToken);

            return new RefreshTokenResult(session.AccessToken, session.RawRefreshToken);

        }
    }
}
