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

        public Task<SessionResult> CreateSessionAsync(User user, DateTime now, CancellationToken cancellationToken = default)
        {
            const int MaxActiveSessions = 5;
            // Limite de sessions — délégué à l'entité
            var activeCount = user.RefreshTokens.Count(t => t.IsActive);
            
            if (activeCount >= MaxActiveSessions)
                user.RevokeOldestActiveSession(now);

            // Génère les tokens
            var tokens = _jwt.GenerateTokens(user.Id, user.Email.Value);

            // Modification de l'entité via sa méthode — DDD pur
            user.AddRefreshToken(tokens.RefreshToken);

            return Task.FromResult(new SessionResult(
                tokens.AccessToken,
                tokens.RawRefreshToken
            ));
        }
    }
}
