using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Forms.Services
{
    public class BlockedUserMiddleware
    {
        private readonly RequestDelegate _next;

        public BlockedUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/Account/SignIn") ||
                context.Request.Path.StartsWithSegments("/Account/Register"))
            {
                await _next(context);
                return;
            }

            if (!context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            if (context.User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var userService = context.RequestServices.GetRequiredService<IUserService>();
                var isBlocked = await userService.UserIsBlocked(userId);

                if (isBlocked)
                {
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    context.Response.Redirect("/Home/Index");
                    return;
                }
            }

            await _next(context);
        }
    }
}
