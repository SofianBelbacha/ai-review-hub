using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Trends.Queries.GetTrends
{
    public class GetTrendsHandler : IRequestHandler<GetTrendsQuery, GetTrendsResult>
    {
        private readonly IAppDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GetTrendsHandler(
            IAppDbContext context,
            ICurrentUserService currentUser,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _currentUser = currentUser;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<GetTrendsResult> Handle(
            GetTrendsQuery request,
            CancellationToken cancellationToken)
        {
            var now = _dateTimeProvider.UtcNow;
            var fromDate = now.AddDays(-request.Days);
            var prevFrom = now.AddDays(-request.Days * 2);

            var query = _context.Feedbacks
                .AsNoTracking()
                .Where(f => f.Project.UserId == _currentUser.UserId);

            if (request.ProjectId.HasValue)
                query = query.Where(f => f.ProjectId == request.ProjectId.Value);

            // Feedbacks de la période
            var periodFeedbacks = await query
                .Where(f => f.CreatedAt >= fromDate)
                .Select(f => new
                {
                    f.CreatedAt,
                    f.Category,
                    f.Priority
                })
                .ToListAsync(cancellationToken);

            // Feedbacks de la période précédente (pour le taux de croissance)
            var previousCount = await query
                .Where(f => f.CreatedAt >= prevFrom && f.CreatedAt < fromDate)
                .CountAsync(cancellationToken);

            // Volume journalier
            var dailyVolume = periodFeedbacks
                .GroupBy(f => f.CreatedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new TrendPoint(
                    g.Key.ToString("yyyy-MM-dd"),
                    g.Count()))
                .ToList();

            // Remplir les jours sans feedbacks
            var allDays = Enumerable
                .Range(0, request.Days)
                .Select(i => fromDate.AddDays(i).Date)
                .ToList();

            var volumeDict = dailyVolume.ToDictionary(d => d.Date, d => d.Count);
            var filledVolume = allDays
                .Select(d => new TrendPoint(
                    d.ToString("yyyy-MM-dd"),
                    volumeDict.GetValueOrDefault(d.ToString("yyyy-MM-dd"), 0)))
                .ToList();

            // Breakdown catégories
            var total = periodFeedbacks.Count;
            var categoryBreakdown = periodFeedbacks
                .GroupBy(f => f.Category)
                .OrderByDescending(g => g.Count())
                .Select(g => new CategoryBreakdown(
                    g.Key.ToString(),
                    g.Count(),
                    total > 0 ? Math.Round(g.Count() * 100.0 / total, 1) : 0))
                .ToList();

            // Breakdown priorités
            var priorityBreakdown = periodFeedbacks
                .GroupBy(f => f.Priority)
                .OrderByDescending(g => g.Count())
                .Select(g => new PriorityBreakdown(
                    g.Key.ToString(),
                    g.Count(),
                    total > 0 ? Math.Round(g.Count() * 100.0 / total, 1) : 0))
                .ToList();

            // Summary
            var peak = filledVolume.MaxBy(d => d.Count);
            var growthRate = previousCount > 0
                ? Math.Round((total - previousCount) * 100.0 / previousCount, 1)
                : total > 0 ? 100.0 : 0.0;

            var summary = new TrendSummary(
                TotalPeriod: total,
                TotalPrevious: previousCount,
                GrowthRate: growthRate,
                AvgPerDay: request.Days > 0
                    ? Math.Round(total / (double)request.Days, 1)
                    : 0,
                PeakCount: peak?.Count ?? 0,
                PeakDate: peak?.Date ?? ""
            );

            return new GetTrendsResult(
                filledVolume,
                categoryBreakdown,
                priorityBreakdown,
                summary);
        }
    }
}
