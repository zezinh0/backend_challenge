using System;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using backend_challenge.Errors;

namespace backend_challenge.Middleware;

/// <summary>
/// Middleware that catches unhandled exceptions.
/// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
/// </summary>
/// <param name="env">The hosting environment used to determine the error response detail visibility.</param>
/// <param name="logger">The logger to log errors.</param>
/// <param name="next">The next middleware in the pipeline.</param>
public class ExceptionMiddleware(IHostEnvironment env, ILogger<ExceptionMiddleware> logger, RequestDelegate next)
{
    /// <summary>
    /// Intercepts HTTP requests and catches any exceptions thrown during processing.
    /// </summary>
    /// <param name="context">The HTTP context representing the current request.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception caught by middleware: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex, env);
        }
    }

    /// <summary>
    /// Handles the exception by formatting a response with an appropriate error message.
    /// In Development, detailed error information is provided; in Production, a generic message is returned.
    /// </summary>
    /// <param name="context">The HTTP context representing the current request.</param>
    /// <param name="ex">The exception that was thrown.</param>
    /// <returns>A task representing the asynchronous operation.</returns> 
    private static Task HandleExceptionAsync(HttpContext context, Exception ex, IHostEnvironment env)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = env.IsDevelopment()
                    ? new ApiErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace)
                    : new ApiErrorResponse(context.Response.StatusCode, "An error occurred. Please try again later.", "Internal Server Error");

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var json = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Extension methods for configuring the exception handling middleware in the application pipeline.
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    /// <summary>
    /// Adds the exception handling middleware to the application's request pipeline.
    /// </summary>
    /// <param name="app">The application builder to configure.</param>
    /// <returns>The application builder with the exception middleware added.</returns>
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}
