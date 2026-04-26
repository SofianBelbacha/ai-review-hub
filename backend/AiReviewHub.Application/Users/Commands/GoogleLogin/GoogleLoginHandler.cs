using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Entities;
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
        private readonly IJwtTokenGenerator _jwt;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GoogleLoginHandler(
            IAppDbContext context,
            IJwtTokenGenerator jwt,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _jwt = jwt;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<GoogleLoginResult> Handle(
            GoogleLoginCommand request,
            CancellationToken cancellationToken)
        {
            var now = _dateTimeProvider.UtcNow;
            var isNewUser = false;

            // Cherche un user existant par email
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u =>
                    u.Email.Value == request.Email.ToLowerInvariant(),
                    cancellationToken);

            if (user is null)
            {
                // Nouvel utilisateur via Google — pas de mot de passe
                user = User.CreateWithGoogle(
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.GoogleId,
                    now
                );

                _context.Users.Add(user);
                isNewUser = true;
            }
            else
            {
                // User existant — lie le compte Google si pas encore fait
                user.LinkGoogleAccount(request.GoogleId, now);
            }

            // Limite de sessions
            var activeTokens = user.RefreshTokens
                .Where(t => t.IsActive)
                .OrderBy(t => t.CreatedAt)
                .ToList();

            if (activeTokens.Count >= 5)
                activeTokens.First().Revoke(now);

            // Génère les tokens
            var tokens = _jwt.GenerateTokens(user.Id, user.Email.Value);
            user.RefreshTokens.Add(tokens.RefreshToken);

            await _context.SaveChangesAsync(cancellationToken);

            return new GoogleLoginResult(
                tokens.AccessToken,
                tokens.RawRefreshToken,
                isNewUser
            );
        }
    }
}
