using AiReviewHub.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Projects.Queries.GetProjectsByUser
{
    public class GetProjectsByUserHandler
        : IRequestHandler<GetProjectsByUserQuery, GetProjectsByUserResult>
    {
        private readonly IAppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetProjectsByUserHandler(
            IAppDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<GetProjectsByUserResult> Handle(
            GetProjectsByUserQuery request,
            CancellationToken cancellationToken)
        {
            var projects = await _context.Projects
                .AsNoTracking()
                .Where(p => p.UserId == _currentUser.UserId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProjectDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.PublicToken,
                    p.IsActive,
                    p.Feedbacks.Count,  // count sans charger les feedbacks
                    p.CreatedAt
                ))
                .ToListAsync(cancellationToken);

            return new GetProjectsByUserResult(projects, projects.Count);
        }
    }
}
