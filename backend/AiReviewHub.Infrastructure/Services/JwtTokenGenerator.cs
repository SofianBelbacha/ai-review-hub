using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AiReviewHub.Infrastructure.Services
{
    public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration = configuration;

        public TokenResult GenerateTokens(Guid userId, string email, string firstName, string lastName, string plan, DateTime now)
        {
            var accessToken = GenerateAccessToken(userId, email, firstName, lastName, plan, now);
            var (refreshTokenEntity, rawToken) = RefreshToken.Create(userId, now);

            return new TokenResult(accessToken, refreshTokenEntity, rawToken);
        }

        private string GenerateAccessToken(Guid userId, string email, string firstName, string lastName, string plan, DateTime now)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expirationHours = _configuration.GetValue<int>("Jwt:ExpirationHours", 2);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim("firstName", firstName),
                new Claim("lastName", lastName),
                new Claim("plan", plan),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: now.AddHours(expirationHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
