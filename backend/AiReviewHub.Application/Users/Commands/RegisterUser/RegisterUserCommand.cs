using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Users.Commands.RegisterUser
{
    public record RegisterUserCommand(
        string Email,
        string Password,
        string FirstName,
        string LastName
    ) : IRequest<RegisterUserResult>;

    public record RegisterUserResult(
        Guid Id,
        string Email,
        string FirstName,
        string LastName,
        string Plan,
        DateTime CreatedAt
    );
}
