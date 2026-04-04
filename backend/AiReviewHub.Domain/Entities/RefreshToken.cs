using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string? ReplacedByToken { get; private set; }

        // Relation
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsExpired && !IsRevoked;

        private RefreshToken() { }

        public static RefreshToken Create(Guid userId, DateTime now)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = GenerateToken(),
                ExpiresAt = now.AddDays(7),
                CreatedAt = now,
                UserId = userId
            };
        }

        public void Revoke(DateTime now, string? replacedByToken = null)
        {
            if (IsRevoked)
                throw new InvalidOperationException("Token is already revoked");

            RevokedAt = now;
            ReplacedByToken = replacedByToken;
        }

        private static string GenerateToken() =>
            Convert.ToBase64String(Guid.NewGuid().ToByteArray()) +
            Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}
