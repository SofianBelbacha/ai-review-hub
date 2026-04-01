using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Feedbacks.Commands.CreateFeedback
{
    public record CreateFeedbackCommand(string Content, Guid ProjectId) : IRequest<CreateFeedbackResult>;

    public record CreateFeedbackResult(Guid Id, string Content, string Category, string Priority, string Status, DateTime CreatedAt);
}
