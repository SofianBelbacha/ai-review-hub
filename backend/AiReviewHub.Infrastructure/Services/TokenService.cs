using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IJwtTokenGenerator _jwt;
        private const int MaxActiveSessions = 5;

        public TokenService(IJwtTokenGenerator jwt)
        {
            _jwt = jwt;
        }

        // TokenService.cs
        public async Task<SessionResult> CreateSessionAsync(
            User user,
            DateTime now,
            IAppDbContext context,
            CancellationToken cancellationToken = default)
        {
            const int MaxActiveSessions = 5;

            var activeCount = user.RefreshTokens.Count(t => t.IsActive);
            if (activeCount >= MaxActiveSessions)
                user.RevokeOldestActiveSession(now);

            var tokens = _jwt.GenerateTokens(user.Id, user.Email.Value);

            // Révocations d'abord
            await context.SaveChangesAsync(cancellationToken);

            // Nouveau token séparément
            context.RefreshTokens.Add(tokens.RefreshToken);
            await context.SaveChangesAsync(cancellationToken);

            return new SessionResult(tokens.AccessToken, tokens.RawRefreshToken);
        }
    }
}
