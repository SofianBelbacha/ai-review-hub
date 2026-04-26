using AiReviewHub.Application.Abstractions;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Infrastructure.Services
{
    public class GoogleTokenValidator : IGoogleTokenValidator
    {
        private readonly IConfiguration _configuration;

        public GoogleTokenValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GoogleUserInfo> ValidateAsync(string idToken)
        {
            GoogleJsonWebSignature.Payload payload;

            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = [_configuration["Google:ClientId"]!]
                    });
            }
            catch (InvalidJwtException ex)
            {
                throw new UnauthorizedAccessException(
                    $"Invalid Google token : {ex.Message}");
            }

            return new GoogleUserInfo(
                payload.Email,
                payload.GivenName ?? string.Empty,
                payload.FamilyName ?? string.Empty,
                payload.Subject,
                payload.EmailVerified
            );
        }
    }
}
