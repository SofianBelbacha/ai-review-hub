using AiReviewHub.Application.Abstractions;
using System.Security.Claims;

namespace AiReviewHub.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                if (user == null || !user.Identity?.IsAuthenticated == true)
                    throw new UnauthorizedAccessException();

                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                return userId is null
                    ? throw new UnauthorizedAccessException()
                    : Guid.Parse(userId);
            }
        }

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
