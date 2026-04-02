using AiReviewHub.Application.Common.Behaviors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Users.Commands.LoginUser
{
    public record LoginUserCommand(
        string Email,
        string Password
    ) : IRequest<LoginUserResult>, ISensitiveRequest;

    public record LoginUserResult(string Token);
}
