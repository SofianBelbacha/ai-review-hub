using AiReviewHub.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Projects.Commands.CreateProject
{
    public class CreateProjectValidator : AbstractValidator<CreateProjectCommand>
    {
        public CreateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required")
                .MaximumLength(Project.MaxNameLength)
                .WithMessage($"Project name cannot exceed {Project.MaxNameLength} characters");

            RuleFor(x => x.Description)
                .MaximumLength(Project.MaxDescriptionLength)
                .WithMessage($"Description cannot exceed {Project.MaxDescriptionLength} characters");
        }
    }
}
