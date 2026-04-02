using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Projects.Commands.CreateProject
{
    public record CreateProjectCommand(string Name, string Description) : IRequest<CreateProjectResult>;

    public record CreateProjectResult(Guid Id, string Name, string Description, string PublicToken, bool IsActive, DateTime CreatedAt);
}
