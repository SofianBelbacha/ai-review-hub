using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Users.Commands.LoginUser
{
    public class LoginUserHandler
        : IRequestHandler<LoginUserCommand, LoginUserResult>
    {
        private readonly IAppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwt;
        IDateTimeProvider _dateTimeProvider;

        public LoginUserHandler(IAppDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwt, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwt = jwt;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email.Value == request.Email, cancellationToken)
                ?? throw new UnauthorizedAccessException("Invalid credentials");

            var isValid = _passwordHasher.Verify(request.Password, user.PasswordHash.Value);

            if (!isValid)
                throw new UnauthorizedAccessException("Invalid credentials");

            var tokens = _jwt.GenerateTokens(user.Id, user.Email.Value);

            // Crée et attache le refresh token
            var refreshToken = RefreshToken.Create(user.Id, _dateTimeProvider.UtcNow);
            user.RefreshTokens.Add(refreshToken);

            // Nettoie les anciens refresh tokens expirés
            var expiredTokens = user.RefreshTokens
                .Where(t => !t.IsActive)
                .ToList();

            foreach (var expired in expiredTokens)
                user.RefreshTokens.Remove(expired);

            await _context.SaveChangesAsync(cancellationToken);


            return new LoginUserResult(tokens.AccessToken, tokens.RefreshToken);
        }
    }
}
