using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Users.Commands.GenerateRefreshToken
{
    public record RefreshTokenCommand(string Token) : IRequest<RefreshTokenResult>;

    public record RefreshTokenResult(string AccessToken, string RefreshToken);
}
