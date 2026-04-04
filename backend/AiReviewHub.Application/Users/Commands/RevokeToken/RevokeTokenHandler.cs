using AiReviewHub.Application.Abstractions;
using AiReviewHub.Domain.Abstractions;
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
        private readonly ICurrentUserService _currentUser;

        public RevokeTokenHandler(
            IAppDbContext context,
            IDateTimeProvider dateTimeProvider,
            ICurrentUserService currentUser)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(
            RevokeTokenCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("User not found");

            var token = user.RefreshTokens
                .FirstOrDefault(t => t.Token == request.Token)
                ?? throw new NotFoundException("Token not found");

            if (!token.IsActive)
                throw new InvalidOperationException("Token is already inactive");

            token.Revoke(_dateTimeProvider.UtcNow);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
