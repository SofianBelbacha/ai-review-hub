using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Projects.Commands.RegenerateProjectToken
{
    public class RegenerateProjectTokenHandler
        : IRequestHandler<RegenerateProjectTokenCommand, RegenerateProjectTokenResult>
    {
        private readonly IAppDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RegenerateProjectTokenHandler(
            IAppDbContext context,
            ICurrentUserService currentUser,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _currentUser = currentUser;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<RegenerateProjectTokenResult> Handle(
            RegenerateProjectTokenCommand request,
            CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p =>
                    p.Id == request.ProjectId &&
                    p.UserId == _currentUser.UserId &&
                    p.IsActive,
                    cancellationToken)
                ?? throw new NotFoundException("Project not found or inactive");

            project.RegenerateToken(_dateTimeProvider);

            await _context.SaveChangesAsync(cancellationToken);

            return new RegenerateProjectTokenResult(project.PublicToken);
        }
    }
}
