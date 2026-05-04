using AiReviewHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Abstractions
{
    public record AiAnalysisResult(
        FeedbackCategory Category,
        FeedbackPriority Priority,
        string Summary
    );

    public interface IAiAnalysisService
    {
        Task<AiAnalysisResult> AnalyzeAsync(
            string content,
            CancellationToken cancellationToken = default);
    }
}
