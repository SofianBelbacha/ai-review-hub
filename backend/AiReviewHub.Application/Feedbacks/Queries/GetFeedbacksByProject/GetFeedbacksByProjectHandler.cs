using AiReviewHub.Application.Abstractions;
using AiReviewHub.Application.Common.Models;
using AiReviewHub.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Feedbacks.Queries.GetFeedbacksByProject
{
    public class GetFeedbacksByProjectHandler : IRequestHandler<GetFeedbacksByProjectQuery, PagedResult<FeedbackDto>>
    {
        private readonly IAppDbContext _context;
        private readonly ICurrentUserService _currentUser;


        public GetFeedbacksByProjectHandler(IAppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<PagedResult<FeedbackDto>> Handle(GetFeedbacksByProjectQuery request, CancellationToken cancellationToken)
        {
            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var page = Math.Max(request.Page, 1);

            // Vérifie que le projet existe

            var projectExists = await _context.Projects
                .AnyAsync(p =>
                    p.Id == request.ProjectId &&
                    p.UserId == _currentUser.UserId,
                    cancellationToken);

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

            var total = await query.CountAsync(cancellationToken);

            var feedbacks = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var dtos = feedbacks
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
                .ToList();

            return new PagedResult<FeedbackDto>(dtos, PaginationMeta.Create(total, page, pageSize));
        }
    }
}
