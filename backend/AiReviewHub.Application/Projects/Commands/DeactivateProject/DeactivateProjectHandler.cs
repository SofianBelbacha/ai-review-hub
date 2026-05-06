using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Projects.Commands.DeactivateProject
{
    public class DeactivateProjectHandler : IRequestHandler<DeactivateProjectCommand, Unit>
    {
        private readonly IAppDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public DeactivateProjectHandler(
            IAppDbContext context,
            ICurrentUserService currentUser,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _currentUser = currentUser;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(
            DeactivateProjectCommand request,
            CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p =>
                    p.Id == request.ProjectId &&
                    p.UserId == _currentUser.UserId,
                    cancellationToken)
                ?? throw new NotFoundException("Project not found");

            project.Deactivate(_dateTimeProvider);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
