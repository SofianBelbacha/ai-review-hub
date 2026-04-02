using AiReviewHub.Application.Abstractions;
using AiReviewHub.Application.Common.Interfaces;
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

        public LoginUserHandler(IAppDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwt)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwt = jwt;
        }

        public async Task<LoginUserResult> Handle(
            LoginUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.Value == request.Email, cancellationToken)
                ?? throw new UnauthorizedAccessException("Invalid credentials");

            var isValid = _passwordHasher.Verify(request.Password, user.PasswordHash.Value);

            if (!isValid)
                throw new UnauthorizedAccessException("Invalid credentials");

            var token = _jwt.GenerateToken(user.Id, user.Email.Value);

            return new LoginUserResult(token);
        }
    }
}
