using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System;
using Forms.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Forms.Handlers
{
    public class ApiTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly FormsDbContext _context;

        public ApiTokenAuthenticationHandler(FormsDbContext context, IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock): base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-API-TOKEN", out var token))
                return AuthenticateResult.Fail("Token missing");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.ApiToken == token);

            if (user == null)
                return AuthenticateResult.Fail("Invalid token");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
