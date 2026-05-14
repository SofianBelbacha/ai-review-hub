using AiReviewHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Abstractions
{
    public record AiAnalysisResult(
        FeedbackCategory Category,
        FeedbackPriority Priority,
        string Summary,
        int? PriorityScore = null,
        string? Sentiment = null,
        int? SentimentScore = null,
        string[]? KeyTopics = null,
        bool? ActionRequired = null,
        string? Urgency = null
    );

    public interface IAiAnalysisService
    {
        Task<AiAnalysisResult> AnalyzeAsync(
            string content,
            Plan plan,
            CancellationToken cancellationToken = default);
    }
}
