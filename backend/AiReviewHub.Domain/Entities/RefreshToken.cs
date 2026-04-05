using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AiReviewHub.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public string TokenHash { get; private set; } = string.Empty; // ← hash en DB
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string? ReplacedByTokenHash { get; private set; }

        // Relation
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsExpired && !IsRevoked;

        private RefreshToken() { }

        public static (RefreshToken Entity, string RawToken) Create(Guid userId, DateTime now)
        {
            var rawToken = GenerateRawToken();
            var entity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                TokenHash = Hash(rawToken),
                ExpiresAt = now.AddDays(7),
                CreatedAt = now,
                UserId = userId
            };

            return (entity, rawToken); // raw renvoyé au client, hash stocké en DB
        }

        public void Revoke(DateTime now, string? replacedByRawToken = null)
        {
            if (IsRevoked)
                throw new InvalidOperationException("Token is already revoked");

            RevokedAt = now;
            ReplacedByTokenHash = replacedByRawToken is null
                ? null
                : Hash(replacedByRawToken);
        }

        public static string Hash(string rawToken)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        private static string GenerateRawToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

    }
}
