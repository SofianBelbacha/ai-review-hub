using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Entities;
using AiReviewHub.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Users.Commands.GoogleLogin
{
    public class GoogleLoginHandler : IRequestHandler<GoogleLoginCommand, GoogleLoginResult>
    {
        private readonly IAppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IGoogleTokenValidator _googleValidator;

        public GoogleLoginHandler(
            IAppDbContext context,
            IDateTimeProvider dateTimeProvider,
            ITokenService tokenService,
            IGoogleTokenValidator googleValidator)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _tokenService = tokenService;
            _googleValidator = googleValidator;
        }

        public async Task<GoogleLoginResult> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {

            // Validation cryptographique du token Google
            var googleUser = await _googleValidator.ValidateAsync(request.IdToken);

            if (!googleUser.EmailVerified)
                throw new UnauthorizedAccessException("Google email is not verified");
            var now = _dateTimeProvider.UtcNow;
            var email = Email.Create(googleUser.Email);
            var isNewUser = false;

            // Cherche un user existant par email
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.GoogleId == googleUser.GoogleId || u.Email == email, cancellationToken);

            if (user is null)
            {
                // Nouvel utilisateur via Google — pas de mot de passe
                user = User.CreateWithGoogle(
                    email.Value,
                    googleUser.FirstName,
                    googleUser.LastName,
                    googleUser.GoogleId,
                    now
                );

                _context.Users.Add(user);
                isNewUser = true;
            }
            else
            {
                // Anti account hijacking — vérifie que le GoogleId correspond
                if (user.GoogleId is not null && user.GoogleId != googleUser.GoogleId)
                    throw new UnauthorizedAccessException("Google account mismatch");

                // User existant — lie le compte Google si pas encore fait
                user.LinkGoogleAccount(googleUser.GoogleId, now);
            }

            // Service — logique métier pure
            var session = _tokenService.PrepareSession(user, now);

            _context.RefreshTokens.Add(session.RefreshTokenEntity);
            await _context.SaveChangesAsync(cancellationToken); 

            return new GoogleLoginResult(
                session.AccessToken,
                session.RawRefreshToken,
                isNewUser
            );
        }
    }
}
