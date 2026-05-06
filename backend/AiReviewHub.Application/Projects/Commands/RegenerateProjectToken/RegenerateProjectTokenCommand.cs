using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Projects.Commands.RegenerateProjectToken
{
    public record RegenerateProjectTokenCommand(Guid ProjectId)
        : IRequest<RegenerateProjectTokenResult>;

    public record RegenerateProjectTokenResult(string PublicToken);
}
