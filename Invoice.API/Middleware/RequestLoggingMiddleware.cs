using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Invoice.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Log incoming request
        var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
        var method = context.Request.Method;
        var path = context.Request.Path;
        var queryString = context.Request.QueryString.ToString();
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        _logger.LogInformation(
            "API Request: {Method} {Path}{QueryString} | User: {UserId} | IP: {IpAddress} | UserAgent: {UserAgent}",
            method, path, queryString, userId, ipAddress, userAgent);

        try
        {
            await _next(context);

            stopwatch.Stop();

            // Log response
            var statusCode = context.Response.StatusCode;
            var logLevel = statusCode >= 400 ? LogLevel.Warning : LogLevel.Information;

            _logger.Log(logLevel,
                "API Response: {Method} {Path}{QueryString} | Status: {StatusCode} | Duration: {Duration}ms | User: {UserId}",
                method, path, queryString, statusCode, stopwatch.ElapsedMilliseconds, userId);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "API Error: {Method} {Path}{QueryString} | Duration: {Duration}ms | User: {UserId} | Error: {ErrorMessage}",
                method, path, queryString, stopwatch.ElapsedMilliseconds, userId, ex.Message);

            throw;
        }
    }
}

