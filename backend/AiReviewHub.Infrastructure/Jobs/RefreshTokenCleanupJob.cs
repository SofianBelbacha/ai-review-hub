using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Infrastructure.Persistence;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Infrastructure.Jobs
{
    public class RefreshTokenCleanupJob
    {
        private readonly AppDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<RefreshTokenCleanupJob> _logger;

        public RefreshTokenCleanupJob(
            AppDbContext context,
            IDateTimeProvider dateTimeProvider,
            ILogger<RefreshTokenCleanupJob> logger)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task CleanupExpiredTokens()
        {
            var cutoff = _dateTimeProvider.UtcNow.AddDays(-30);

            var deleted = await _context.RefreshTokens
                .Where(t =>
                    (t.RevokedAt != null && t.RevokedAt < cutoff) ||
                    t.ExpiresAt < cutoff)
                .ExecuteDeleteAsync();

            _logger.LogInformation(
                "Cleaned up {Count} expired refresh tokens", deleted);
        }
    }
}
