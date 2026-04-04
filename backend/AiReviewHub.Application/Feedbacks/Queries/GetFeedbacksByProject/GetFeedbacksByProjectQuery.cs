using AiReviewHub.Application.Common.Models;
using AiReviewHub.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Feedbacks.Queries.GetFeedbacksByProject
{
    public record GetFeedbacksByProjectQuery(
        Guid ProjectId,
        FeedbackStatus? StatusFilter = null,
        FeedbackCategory? CategoryFilter = null,
        FeedbackPriority? PriorityFilter = null,
        int Page = 1,
        int PageSize = 20
    ) : IRequest<PagedResult<FeedbackDto>>;

    public record GetFeedbacksByProjectResult(
        IReadOnlyList<FeedbackDto> Feedbacks,
        int TotalCount
    );

    public record FeedbackDto(
        Guid Id,
        string Content,
        string AiSummary,
        string Category,
        string Priority,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
