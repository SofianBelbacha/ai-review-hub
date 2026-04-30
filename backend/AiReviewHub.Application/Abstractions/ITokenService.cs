using AiReviewHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Abstractions
{
    public record SessionResult(string AccessToken, string RawRefreshToken);

    public interface ITokenService
    {
        Task<SessionResult> CreateSessionAsync(
            User user,
            DateTime now,
            IAppDbContext context,
            CancellationToken cancellationToken = default);
        
    }
}
