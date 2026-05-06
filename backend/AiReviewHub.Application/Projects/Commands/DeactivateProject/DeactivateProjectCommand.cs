using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace AiReviewHub.Application.Projects.Commands.DeactivateProject
{
    public record DeactivateProjectCommand(Guid ProjectId) : IRequest<Unit>;
}

