using Microsoft.AspNetCore.Http;

namespace TSP.Controller.Extensions
{
    public static class CookieExtension
    {
        public static IResponseCookies SetRefreshTokenCookie(this IResponseCookies cookies, string refreshToken, DateTime expiresAt)
        {
            cookies.Append("refreshToken", refreshToken, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,

                Expires = new DateTimeOffset(
                    DateTime.SpecifyKind(
                        expiresAt,
                        DateTimeKind.Utc)),

                Path = "/api/auth",

                IsEssential = true
            });

            return cookies;
        }

        public static IResponseCookies DeleteRefreshTokenCookie(this IResponseCookies cookies)
        {
            cookies.Delete("refreshToken", new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "api/auth"
            });

            return cookies;
        }

        public static string? GetRefreshTokenCookie(this IRequestCookieCollection cookies)
        {
            var refreshToken = cookies["refreshToken"];

            return refreshToken;
        }
    }
}
