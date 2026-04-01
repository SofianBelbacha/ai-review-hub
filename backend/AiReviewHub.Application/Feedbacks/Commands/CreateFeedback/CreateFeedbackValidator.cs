using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Feedbacks.Commands.CreateFeedback
{
    public class CreateFeedbackValidator : AbstractValidator<CreateFeedbackCommand>
    {
        public CreateFeedbackValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required")
                .MinimumLength(10).WithMessage("Content must be at least 10 characters")
                .MaximumLength(5000).WithMessage("Content cannot exceed 5000 characters");

            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("ProjectId is required");
        }
    }
}
