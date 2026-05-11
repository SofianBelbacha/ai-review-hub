using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Abstractions
{
    public interface IGoogleTokenValidator
    {
        Task<GoogleUserInfo> ValidateAsync(string idToken);
    }

    public record GoogleUserInfo(
        string Email,
        string GoogleId,
        bool EmailVerified,
        string? FirstName = null,
        string? LastName = null,
        string? FullName = null

    );
}
