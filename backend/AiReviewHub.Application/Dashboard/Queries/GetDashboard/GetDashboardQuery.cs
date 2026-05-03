using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Dashboard.Queries.GetDashboard
{
    public record GetDashboardQuery(Guid? ProjectId = null)
        : IRequest<GetDashboardResult>;

    public record GetDashboardResult(
        DashboardStats Stats,
        IReadOnlyList<TrendPoint> Trends,
        IReadOnlyList<DashboardFeedbackDto> RecentFeedbacks
    );

    public record DashboardStats(
        int TotalFeedbacks,
        int TodoCount,
        int InProgressCount,
        int ResolvedCount,
        int HighPriorityCount
    );

    public record TrendPoint(string Date, int Count);

    public record DashboardFeedbackDto(
        Guid Id,
        string Content,
        string AiSummary,
        string Category,
        string Priority,
        string Status,
        DateTime CreatedAt
    );
}
