namespace ZuluVault.Api.Middleware;

/// <summary>
/// Request logging middleware
/// </summary>
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
        // Log request
        _logger.LogInformation(
            "Incoming Request: {Method} {Path}",
            context.Request.Method,
            context.Request.Path
        );

        // Log response
        var originalBodyStream = context.Response.Body;

        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            await _next(context);

            _logger.LogInformation(
                "Response: {StatusCode} {Method} {Path}",
                context.Response.StatusCode,
                context.Request.Method,
                context.Request.Path
            );

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}

public static class RequestLoggingExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
