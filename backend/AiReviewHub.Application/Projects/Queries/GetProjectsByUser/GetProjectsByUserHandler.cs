using AiReviewHub.Application.Abstractions;
using AiReviewHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Projects.Queries.GetProjectsByUser
{
    public class GetProjectsByUserHandler : IRequestHandler<GetProjectsByUserQuery, PagedResult<ProjectDto>>
    {
        private readonly IAppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetProjectsByUserHandler(IAppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<PagedResult<ProjectDto>> Handle(GetProjectsByUserQuery request, CancellationToken cancellationToken)
        {
            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var page = Math.Max(request.Page, 1);

            var query = _context.Projects
                .AsNoTracking()
                .Where(p => p.UserId == _currentUser.UserId)
                .OrderByDescending(p => p.CreatedAt);

            var total = await query.CountAsync(cancellationToken);

            var projects = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProjectDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.PublicToken,
                    p.IsActive,
                    p.Feedbacks.Count,
                    p.CreatedAt
                ))
                .ToListAsync(cancellationToken);


            return new PagedResult<ProjectDto>(projects, PaginationMeta.Create(total, page, pageSize)
);
        }
    }
}
