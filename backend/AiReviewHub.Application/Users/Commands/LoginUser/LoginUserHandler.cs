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
        private readonly ITokenService _tokenService;


        public LoginUserHandler(IAppDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwt, IDateTimeProvider dateTimeProvider, ITokenService tokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwt = jwt;
            _dateTimeProvider = dateTimeProvider;
            _tokenService = tokenService;
        }

        public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email.Value == request.Email.ToLowerInvariant(), cancellationToken)
                ?? throw new UnauthorizedAccessException("Invalid credentials");

            if (user.IsOAuthUser)
                throw new UnauthorizedAccessException(
                    "This account uses Google Sign-In. Please use Google to login.");

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash!.Value))
                throw new UnauthorizedAccessException("Invalid credentials");

            // Délègue la création de session à ITokenService
            var session = await _tokenService.CreateSessionAsync(
                user, _dateTimeProvider.UtcNow, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);


            return new LoginUserResult(session.AccessToken, session.RawRefreshToken);
        }
    }
}
