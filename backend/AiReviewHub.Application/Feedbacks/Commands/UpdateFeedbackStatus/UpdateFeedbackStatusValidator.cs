using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Feedbacks.Commands.UpdateFeedbackStatus
{
    public class UpdateFeedbackStatusValidator : AbstractValidator<UpdateFeedbackStatusCommand>
    {
        public UpdateFeedbackStatusValidator()
        {
            RuleFor(x => x.FeedbackId)
                .NotEmpty().WithMessage("FeedbackId is required");

            RuleFor(x => x.NewStatus)
                .IsInEnum().WithMessage("Invalid status value");
        }
    }
}
