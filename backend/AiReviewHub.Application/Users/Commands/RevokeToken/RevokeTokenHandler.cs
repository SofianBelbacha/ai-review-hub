using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
using AiReviewHub.Domain.Entities;
using AiReviewHub.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Users.Commands.RevokeToken
{
    public class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand, Unit>
    {
        private readonly IAppDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RevokeTokenHandler(IAppDbContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(
            RevokeTokenCommand request,
            CancellationToken cancellationToken)
        {
            var now = _dateTimeProvider.UtcNow;

            if (request.RevokeAll)
            {
                // Pour revokeAll on a besoin du user — via le token
                var tokenHash = RefreshToken.Hash(request.Token);

                var userForRevoke = await _context.Users
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u =>
                        u.RefreshTokens.Any(t => t.TokenHash == tokenHash),
                        cancellationToken)
                    ?? throw new NotFoundException("User not found");

                foreach (var t in userForRevoke.RefreshTokens.Where(t => t.IsActive))
                    t.Revoke(now);

                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }

            // Révocation simple — cherche directement le token
            var hash = RefreshToken.Hash(request.Token);

            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken)
                ?? throw new NotFoundException("Token not found");

            if (!token.IsActive)
                throw new InvalidOperationException("Token is already inactive");

            token.Revoke(now);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
