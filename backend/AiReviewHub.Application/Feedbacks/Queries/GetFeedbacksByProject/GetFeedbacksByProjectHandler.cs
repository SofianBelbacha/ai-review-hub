using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Feedbacks.Queries.GetFeedbacksByProject
{
    public class GetFeedbacksByProjectHandler
        : IRequestHandler<GetFeedbacksByProjectQuery, GetFeedbacksByProjectResult>
    {
        private readonly IAppDbContext _context;

        public GetFeedbacksByProjectHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<GetFeedbacksByProjectResult> Handle(
            GetFeedbacksByProjectQuery request,
            CancellationToken cancellationToken)
        {
            // Vérifie que le projet existe
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);

            if (!projectExists)
                throw new NotFoundException($"Project {request.ProjectId} not found");

            // Construction de la query avec filtres optionnels
            var query = _context.Feedbacks
                .AsNoTracking()
                .Where(f => f.ProjectId == request.ProjectId);

            if (request.StatusFilter.HasValue)
                query = query.Where(f => f.Status == request.StatusFilter.Value);

            if (request.CategoryFilter.HasValue)
                query = query.Where(f => f.Category == request.CategoryFilter.Value);

            if (request.PriorityFilter.HasValue)
                query = query.Where(f => f.Priority == request.PriorityFilter.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var feedbacks = await query
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FeedbackDto(
                    f.Id,
                    f.Content.Value,
                    f.AiSummary,
                    f.Category.ToString(),
                    f.Priority.ToString(),
                    f.Status.ToString(),
                    f.CreatedAt,
                    f.UpdatedAt
                ))
                .ToListAsync(cancellationToken);

            return new GetFeedbacksByProjectResult(feedbacks, totalCount);
        }
    }
}
