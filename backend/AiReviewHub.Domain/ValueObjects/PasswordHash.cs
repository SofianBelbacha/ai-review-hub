using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Domain.ValueObjects
{
    public sealed class PasswordHash
    {
        // Préfixes bcrypt valides
        private static readonly string[] ValidPrefixes = ["$2a$", "$2b$", "$2x$", "$2y$"];

        public string Value { get; }

        private PasswordHash(string value) => Value = value;

        public static PasswordHash Create(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException("Password hash cannot be empty");

            if (!ValidPrefixes.Any(prefix => hash.StartsWith(prefix)))
                throw new ArgumentException("Password hash must be a valid bcrypt hash");

            // Bcrypt produit toujours 60 caractères
            if (hash.Length != 60)
                throw new ArgumentException("Password hash has an invalid length");

            return new PasswordHash(hash);
        }

        public override string ToString() => Value;
        public override bool Equals(object? obj) => obj is PasswordHash p && p.Value == Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}
