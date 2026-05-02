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

        public SessionResult PrepareSession(User user, DateTime now)
        {
            // Limite de sessions — logique métier pure
            var activeTokens = user.RefreshTokens
                .Where(t => t.IsActive)
                .OrderBy(t => t.CreatedAt)
                .ToList();

            if (activeTokens.Count >= MaxActiveSessions)
                user.RevokeOldestActiveSession(now);

            // Génère les tokens
            var tokens = _jwt.GenerateTokens(user.Id, user.Email.Value);

            return new SessionResult(
                tokens.AccessToken,
                tokens.RawRefreshToken,
                tokens.RefreshToken  // entité retournée
            );
        }
    }
}
