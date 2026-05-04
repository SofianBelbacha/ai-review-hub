using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Entities;
using AiReviewHub.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Feedbacks.Commands.CreateFeedback
{

    public class CreateFeedbackHandler : IRequestHandler<CreateFeedbackCommand, CreateFeedbackResult>
    {
        private readonly IAppDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;
        private readonly IBackgroundJobClient _backgroundJobs;


        public CreateFeedbackHandler(IAppDbContext context, IDateTimeProvider dateTimeProvider, ICurrentUserService currentUser,IMapper mapper, IBackgroundJobClient backgroundJobs)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _currentUser = currentUser;
            _mapper = mapper;
            _backgroundJobs = backgroundJobs;
        }

        public async Task<CreateFeedbackResult> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
        {
            // Vérifie que le projet existe
            var project = await _context.Projects
                .FirstOrDefaultAsync(p =>
                    p.Id == request.ProjectId &&
                    p.UserId == _currentUser.UserId,
                    cancellationToken)
                ?? throw new NotFoundException("Project not found");

            // Crée le feedback via le Domain
            var feedback = Feedback.Create(
                request.Content,
                request.ProjectId,
                _dateTimeProvider.UtcNow
            );

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CreateFeedbackResult>(feedback);
        }
    }
}
