using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Feedbacks.Commands.UpdateFeedbackStatus
{
    public class UpdateFeedbackStatusHandler
        : IRequestHandler<UpdateFeedbackStatusCommand, Unit>
    {
        private readonly IAppDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUser;


        public UpdateFeedbackStatusHandler(IAppDbContext context, IDateTimeProvider dateTimeProvider, ICurrentUserService currentUser)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(
            UpdateFeedbackStatusCommand request,
            CancellationToken cancellationToken)
        {
            var feedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.Id == request.FeedbackId && f.ProjectId == request.ProjectId &&
                    f.Project.UserId == _currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException($"Feedback {request.FeedbackId} not found");

            // La logique de transition est dans le Domain
            feedback.UpdateStatus(request.NewStatus, _dateTimeProvider.UtcNow);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
