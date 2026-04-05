using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Users.Commands.RevokeToken
{
    public record RevokeTokenCommand(string Token, bool RevokeAll = false) : IRequest<Unit>;
}
