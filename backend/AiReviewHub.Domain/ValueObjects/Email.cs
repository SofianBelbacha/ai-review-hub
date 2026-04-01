using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Domain.ValueObjects
{
    public sealed class Email
    {
        public string Value { get; }

        private Email(string value) => Value = value;

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty");

            value = value.Trim().ToLowerInvariant();

            if (!value.Contains('@') || value.Length > 254)
                throw new ArgumentException("Email is invalid");

            return new Email(value);
        }

        public override string ToString() => Value;
        public override bool Equals(object? obj) => obj is Email e && e.Value == Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}
