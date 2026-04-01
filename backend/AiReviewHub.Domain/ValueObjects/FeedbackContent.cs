using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Domain.ValueObjects
{
    public sealed class FeedbackContent
    {
        public const int MaxLength = 5000;
        public const int MinLength = 10;

        public string Value { get; }

        private FeedbackContent(string value) => Value = value;

        public static FeedbackContent Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Content cannot be empty");

            value = value.Trim();

            if (value.Length < MinLength)
                throw new ArgumentException($"Content must be at least {MinLength} characters");

            if (value.Length > MaxLength)
                throw new ArgumentException($"Content cannot exceed {MaxLength} characters");

            return new FeedbackContent(value);
        }

        public override string ToString() => Value;
    }
}
