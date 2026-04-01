using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Domain.ValueObjects
{
    public sealed class PasswordHash
    {
        public string Value { get; }

        private PasswordHash(string value) => Value = value;

        public static PasswordHash Create(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException("Password hash cannot be empty");

            return new PasswordHash(hash);
        }

        public override string ToString() => Value;
    }
}
