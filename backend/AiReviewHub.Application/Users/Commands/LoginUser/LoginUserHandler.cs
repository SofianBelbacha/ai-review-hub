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

            if (user.IsOAuthUser)
                throw new UnauthorizedAccessException(
                    "This account uses Google Sign-In. Please use Google to login.");

            var isValid = _passwordHasher.Verify(request.Password, user.PasswordHash!.Value);

            if (!isValid)
                throw new UnauthorizedAccessException("Invalid credentials");

            var tokens = _jwt.GenerateTokens(user.Id, user.Email.Value);

            // On attache l'objet RefreshToken directement
            user.RefreshTokens.Add(tokens.RefreshToken);

            await _context.SaveChangesAsync(cancellationToken);


            return new LoginUserResult(tokens.AccessToken, tokens.RawRefreshToken);
        }
    }
}
