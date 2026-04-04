using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Abstractions
{
    public record TokenResult(string AccessToken, string RefreshToken);

    public interface IJwtTokenGenerator
    {
        TokenResult GenerateTokens(Guid userId, string email);
    }
}
