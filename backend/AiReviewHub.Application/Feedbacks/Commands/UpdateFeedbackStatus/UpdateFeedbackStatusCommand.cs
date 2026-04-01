using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Enums;
using AiReviewHub.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Feedbacks.Commands.UpdateFeedbackStatus
{
    public record UpdateFeedbackStatusCommand(
        Guid FeedbackId,
        FeedbackStatus NewStatus
    ) : IRequest<Unit>;
}
