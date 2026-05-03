using AiReviewHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Abstractions
{
    public record TokenResult(string AccessToken, RefreshToken RefreshToken, string RawRefreshToken);

    public interface IJwtTokenGenerator
    {
        TokenResult GenerateTokens(Guid userId, string email, string firstName, string lastName, string plan, DateTime now);
    }
}
