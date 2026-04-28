namespace AiReviewHub.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static void SetRefreshTokenCookie(
        this HttpContext context,
        string refreshToken)
        {
            context.Response.Cookies.Append("refresh_token", refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,                          // inaccessible depuis JS
                    Secure = true,                          // HTTPS uniquement
                    SameSite = SameSiteMode.Lax,           // protection CSRF
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    Path = "/api/auth",                   // limité aux routes auth
                    IsEssential = true
                });
        }

        public static void ClearRefreshTokenCookie(this HttpContext context)
        {
            context.Response.Cookies.Delete("refresh_token",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Path = "/api/auth"
                });
        }
    }
}
