using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Trends.Queries.GetTrends
{
    public record GetTrendsQuery(
        int Days = 30,
        Guid? ProjectId = null
    ) : IRequest<GetTrendsResult>;

    public record GetTrendsResult(
        IReadOnlyList<TrendPoint> DailyVolume,
        IReadOnlyList<CategoryBreakdown> CategoryBreakdown,
        IReadOnlyList<PriorityBreakdown> PriorityBreakdown,
        TrendSummary Summary
    );

    public record TrendPoint(string Date, int Count);
    public record CategoryBreakdown(string Category, int Count, double Percentage);
    public record PriorityBreakdown(string Priority, int Count, double Percentage);
    public record TrendSummary(
        int TotalPeriod,
        int TotalPrevious,
        double GrowthRate,
        double AvgPerDay,
        int PeakCount,
        string PeakDate
    );
}
