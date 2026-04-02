using AiReviewHub.Application.Abstractions;
using AiReviewHub.Application.Common.Interfaces;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Entities;
using AiReviewHub.Domain.Exceptions;
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
        private readonly IJwtTokenGenerator _jwt;
        private readonly IMapper _mapper;

        public RegisterUserHandler(IAppDbContext context,
            IDateTimeProvider dateTimeProvider,
            IPasswordHasher passwordHasher,
            IMapper mapper, IJwtTokenGenerator jwt)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _jwt = jwt;
        }

        public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Vérifie que l'email n'est pas déjà utilisé
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email.Value == request.Email,
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
            await _context.SaveChangesAsync(cancellationToken);

            var token = _jwt.GenerateToken(user.Id, user.Email.Value);

            return _mapper.Map<RegisterUserResult>(token);
        }
    }
}
