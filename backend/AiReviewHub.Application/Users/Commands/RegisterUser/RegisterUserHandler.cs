using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Entities;
using AiReviewHub.Domain.Exceptions;
using AiReviewHub.Domain.ValueObjects;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Users.Commands.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
    {
        private readonly IAppDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IJwtTokenGenerator _jwt;
        private readonly IMapper _mapper;

        public RegisterUserHandler(IAppDbContext context,
            IDateTimeProvider dateTimeProvider,
            IPasswordHasher passwordHasher,
            IMapper mapper,  IJwtTokenGenerator jwt, ITokenService tokenService)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _jwt = jwt;
            _tokenService = tokenService;
        }

        public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Vérifie que l'email n'est pas déjà utilisé
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == Email.Create(request.Email),
                    cancellationToken);

            if (emailExists)
                throw new ConflictException("Email is already in use");

            // Hash du mot de passe via l'abstraction
            var passwordHash = _passwordHasher.Hash(request.Password);

            // Création via le Domain
            var user = User.Create(
                request.Email,
                passwordHash,
                request.FirstName,
                request.LastName,
                _dateTimeProvider.UtcNow
            );


            _context.Users.Add(user);

            // Délègue la création de session à ITokenService
            var session = await _tokenService.CreateSessionAsync(user, _dateTimeProvider.UtcNow, _context, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return new RegisterUserResult(
                user.Id,
                user.Email.Value,
                user.FirstName,
                user.LastName,
                user.Plan.ToString(),
                session.AccessToken,
                session.RawRefreshToken,
                user.CreatedAt
            );
        }
    }
}
