using AiReviewHub.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Tests.Helpers
{
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow { get; set; } = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);
    }
}
