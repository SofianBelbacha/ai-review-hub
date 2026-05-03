using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Dashboard.Queries.GetDashboard
{
    public class GetDashboardHandler
        : IRequestHandler<GetDashboardQuery, GetDashboardResult>
    {
        private readonly IAppDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GetDashboardHandler(
            IAppDbContext context,
            ICurrentUserService currentUser,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _currentUser = currentUser;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<GetDashboardResult> Handle(
            GetDashboardQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            var now = _dateTimeProvider.UtcNow;
            var from = now.AddDays(-30);

            // Base query — feedbacks des projets de l'utilisateur
            var feedbacksQuery = _context.Feedbacks
                .AsNoTracking()
                .Where(f => f.Project.UserId == userId);

            // Filtre par projet si spécifié
            if (request.ProjectId.HasValue)
                feedbacksQuery = feedbacksQuery
                    .Where(f => f.ProjectId == request.ProjectId.Value);

            // Charge tout en mémoire pour les stats
            var feedbacks = await feedbacksQuery
                .Select(f => new
                {
                    f.Status,
                    f.Priority,
                    f.CreatedAt
                })
                .ToListAsync(cancellationToken);

            // Stats
            var stats = new DashboardStats(
                TotalFeedbacks: feedbacks.Count,
                TodoCount: feedbacks.Count(f => f.Status == FeedbackStatus.Todo),
                InProgressCount: feedbacks.Count(f => f.Status == FeedbackStatus.InProgress),
                ResolvedCount: feedbacks.Count(f => f.Status == FeedbackStatus.Done),
                HighPriorityCount: feedbacks.Count(f =>
                    f.Priority == FeedbackPriority.High ||
                    f.Priority == FeedbackPriority.Critical)
            );

            // Tendance 30 jours
            var trends = feedbacks
                .Where(f => f.CreatedAt >= from)
                .GroupBy(f => f.CreatedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new TrendPoint(
                    g.Key.ToString("yyyy-MM-dd"),
                    g.Count()))
                .ToList();

            // Feedbacks récents avec détails
            var recentFeedbacks = await feedbacksQuery
                .OrderByDescending(f => f.CreatedAt)
                .Take(30)
                .ToListAsync(cancellationToken);

            var dtos = recentFeedbacks
                .Select(f => new DashboardFeedbackDto(
                    f.Id,
                    f.Content.Value,
                    f.AiSummary,
                    f.Category.ToString(),
                    f.Priority.ToString(),
                    f.Status.ToString(),
                    f.CreatedAt
                ))
                .ToList();

            return new GetDashboardResult(stats, trends, dtos);
        }
    }
}
