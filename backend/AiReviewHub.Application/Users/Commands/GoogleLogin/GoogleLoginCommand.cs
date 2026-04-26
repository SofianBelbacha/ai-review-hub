using AiReviewHub.Application.Common.Behaviors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Users.Commands.GoogleLogin
{
    public record GoogleLoginCommand(string IdToken)
        : IRequest<GoogleLoginResult>, ISensitiveRequest;

    public record GoogleLoginResult(
        string AccessToken,
        string RefreshToken,
        bool IsNewUser
    );
}
