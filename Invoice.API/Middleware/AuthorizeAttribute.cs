using Invoice.Domain.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Invoice.API.Middleware;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        var user = (UserDto)context.HttpContext.Items["User"];
        if (user == null)
            context.Result = new JsonResult(new { message = "Unauthorized" })
            { StatusCode = StatusCodes.Status401Unauthorized };
    }
}

