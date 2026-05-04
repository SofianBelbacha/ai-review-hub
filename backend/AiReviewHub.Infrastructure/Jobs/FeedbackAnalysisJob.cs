using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Enums;
using AiReviewHub.Infrastructure.Persistence;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Infrastructure.Jobs
{
    public class FeedbackAnalysisJob
    {
        private readonly IAppDbContext _context;
        private readonly IAiAnalysisService _aiService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<FeedbackAnalysisJob> _logger;

        public FeedbackAnalysisJob(
            IAppDbContext context,
            IAiAnalysisService aiService,
            IDateTimeProvider dateTimeProvider,
            ILogger<FeedbackAnalysisJob> logger)
        {
            _context = context;
            _aiService = aiService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = [30, 60, 120])]
        public async Task AnalyzeFeedbackAsync(Guid feedbackId)
        {
            var now = _dateTimeProvider.UtcNow;

            var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == feedbackId);

            if (feedback is null)
            {
                _logger.LogWarning(
                    "[AI] Feedback {FeedbackId} not found — skipping", feedbackId);
                return;
            }

            if (feedback.AiAnalysisStatus == AiAnalysisStatus.Completed)
            {
                _logger.LogInformation(
                    "[AI] Feedback {FeedbackId} already analyzed — skipping", feedbackId);
                return;
            }

            _logger.LogInformation("[AI] Analyzing feedback {FeedbackId}", feedbackId);

            feedback.MarkAsProcessing(now);
            await ((AppDbContext)_context).SaveChangesAsync();

            try
            {
                var result = await _aiService.AnalyzeAsync(feedback.Content.Value);

                feedback.EnrichWithAi(
                    result.Category,
                    result.Priority,
                    result.Summary,
                    _dateTimeProvider.UtcNow
                );

                await ((AppDbContext)_context).SaveChangesAsync();

                _logger.LogInformation(
                    "[AI] Feedback {FeedbackId} analyzed — {Category} / {Priority}",
                    feedbackId, result.Category, result.Priority);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[AI] Failed to analyze feedback {FeedbackId}", feedbackId);

                feedback.MarkAsFailed(ex.Message, _dateTimeProvider.UtcNow);
                await ((AppDbContext)_context).SaveChangesAsync();

                throw; // Hangfire gère le retry
            }
        }
    }
}
