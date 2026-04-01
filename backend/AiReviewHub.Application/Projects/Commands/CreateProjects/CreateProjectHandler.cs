using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Entities;
using AiReviewHub.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Projects.Commands.CreateProjects
{
    public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, CreateProjectResult>
    {
        private readonly IAppDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public CreateProjectHandler(
            IAppDbContext context,
            IDateTimeProvider dateTimeProvider,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<CreateProjectResult> Handle(
            CreateProjectCommand request,
            CancellationToken cancellationToken)
        {
            // Vérifie que l'utilisateur existe
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("User not found");

            // Limite freemium — max 1 projet en plan Free
            var projectCount = await _context.Projects
                .CountAsync(p => p.UserId == _currentUser.UserId && p.IsActive, cancellationToken);

            if (user.Plan == Domain.Enums.Plan.Free && projectCount >= 1)
                throw new ForbiddenException("Free plan allows only 1 active project. Upgrade to Pro.");

            if (user.Plan == Domain.Enums.Plan.Pro && projectCount >= 10)
                throw new ForbiddenException("Pro plan allows up to 10 active projects. Upgrade to Team.");

            // Création via le Domain
            var project = Project.Create(
                request.Name,
                request.Description,
                _currentUser.UserId,
                _dateTimeProvider.UtcNow
            );

            _context.Projects.Add(project);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CreateProjectResult>(project);
        }
    }
}
