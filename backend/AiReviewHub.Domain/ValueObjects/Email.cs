using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Domain.ValueObjects
{
    public sealed record Email
    {
        public string Value { get; }

        private Email(string value) => Value = value;

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty");

            value = value.Trim().ToLowerInvariant();

            if (value.Length > 254)
                throw new ArgumentException("Email cannot exceed 254 characters");

            try
            {
                var addr = new System.Net.Mail.MailAddress(value);
                if (addr.Address != value)
                    throw new ArgumentException("Email is invalid");
            }
            catch (FormatException)
            {
                throw new ArgumentException("Email is invalid");
            }

            return new Email(value.Trim().ToLowerInvariant());
        }

        public override string ToString() => Value;
    }

}
