using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace Invoice.API.Middleware;

public class ContextUserAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public ContextUserAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.User?.Identity != null && Context.User.Identity.IsAuthenticated)
        {
            var ticket = new AuthenticationTicket(Context.User, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.NoResult());
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }
}


