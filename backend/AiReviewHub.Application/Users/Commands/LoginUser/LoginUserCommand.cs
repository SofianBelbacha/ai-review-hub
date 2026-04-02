using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Users.Commands.LoginUser
{
    public record LoginUserCommand(
        string Email,
        string Password
    ) : IRequest<LoginUserResult>;

    public record LoginUserResult(string Token);
}
