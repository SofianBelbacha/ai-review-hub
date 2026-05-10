using AiReviewHub.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Feedbacks.Queries.ExportFeedbacksCsv
{
    public record ExportFeedbacksCsvQuery(
        Guid ProjectId,
        FeedbackStatus? StatusFilter = null,
        FeedbackCategory? CategoryFilter = null,
        FeedbackPriority? PriorityFilter = null
    ) : IRequest<ExportFeedbacksCsvResult>;

    public record ExportFeedbacksCsvResult(
        byte[] Content,
        string FileName
    );
}
