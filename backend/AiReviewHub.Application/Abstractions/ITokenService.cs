using AiReviewHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Abstractions
{
    public record SessionResult(string AccessToken, string RawRefreshToken, RefreshToken RefreshTokenEntity);

    public interface ITokenService
    {
        SessionResult PrepareSession(User user, DateTime now);
        
    }
}
