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
        string FirstName,
        string LastName,
        string GoogleId,
        bool EmailVerified
    );
}
