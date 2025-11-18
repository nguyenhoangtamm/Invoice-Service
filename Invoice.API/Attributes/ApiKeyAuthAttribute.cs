using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Invoice.API.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
{
    private const string ApiKeyHeaderName = "X-API-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Check if API key is provided in header
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyValues))
        {
            context.Result = new UnauthorizedObjectResult(Result<object>.Failure("API key is required"));
            return;
        }

        var apiKey = apiKeyValues.FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
        {
            context.Result = new UnauthorizedObjectResult(Result<object>.Failure("API key is required"));
            return;
        }

        // Validate the API key
        var apiKeyService = context.HttpContext.RequestServices.GetRequiredService<IApiKeyService>();
        var validationResult = await apiKeyService.ValidateApiKey(apiKey, context.HttpContext.RequestAborted);

        if (!validationResult.Succeeded)
        {
            context.Result = new UnauthorizedObjectResult(Result<object>.Failure(validationResult.Message));
            return;
        }

        // Store the validated API key info in HttpContext for later use
        context.HttpContext.Items["ApiKeyInfo"] = validationResult.Data;
        context.HttpContext.Items["OrganizationId"] = validationResult.Data?.OrganizationId;

        await next();
    }
}