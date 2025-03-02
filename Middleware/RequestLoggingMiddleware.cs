using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace backend_challenge.Middleware;

/// <summary>
/// Middleware that logs the details of incoming HTTP requests and their responses.
/// Logs the request method, path, query string, response status code, and the duration of the request.
/// </summary>
public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private readonly RequestDelegate next = next;
    private readonly ILogger<RequestLoggingMiddleware> logger = logger;

    /// <summary>
    /// Intercepts the HTTP request, logs the request details, and measures the request processing time.
    /// Logs the request completion along with the response status and time taken to process the request.
    /// </summary>
    /// <param name="context">The HTTP context representing the current request.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Start timing the request
        var stopwatch = Stopwatch.StartNew();

        logger.LogDebug(
            "Request started: {Method} {Path}{QueryString}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString);

        var originalBodyStream = context.Response.Body;

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Log the response details
            logger.LogDebug(
                "Request completed: {Method} {Path}{QueryString} - Status: {StatusCode} - Duration: {ElapsedMilliseconds}ms",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
/// <summary>
/// Extension methods for the middleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds the request logging middleware to the application's request pipeline.
    /// </summary>
    /// <param name="app">The application builder used to configure the request pipeline.</param>
    /// <returns>The application builder with the request logging middleware added.</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}