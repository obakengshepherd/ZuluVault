using System.Net;
using System.Text.Json;
using ZuluVault.Domain.Common;

namespace ZuluVault.Api.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message = exception.Message,
            timestamp = DateTime.UtcNow
        };

        return exception switch
        {
            DomainException domainEx =>
                HandleDomainException(context, domainEx, response),
            InvalidWalletOperationException invalidOp =>
                HandleInvalidOperation(context, invalidOp, response),
            InsufficientFundsException insufficientFunds =>
                HandleInsufficientFunds(context, insufficientFunds, response),
            DailyLimitExceededException dailyLimit =>
                HandleDailyLimitExceeded(context, dailyLimit, response),
            WalletLockedException walletLocked =>
                HandleWalletLocked(context, walletLocked, response),
            _ => HandleGenericException(context, exception, response)
        };
    }

    private static Task HandleDomainException(HttpContext context, DomainException exception, dynamic response)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleInvalidOperation(HttpContext context, InvalidWalletOperationException exception, dynamic response)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleInsufficientFunds(HttpContext context, InsufficientFundsException exception, dynamic response)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleDailyLimitExceeded(HttpContext context, DailyLimitExceededException exception, dynamic response)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleWalletLocked(HttpContext context, WalletLockedException exception, dynamic response)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleGenericException(HttpContext context, Exception exception, dynamic response)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        response.message = "An internal server error occurred";
        return context.Response.WriteAsJsonAsync(response);
    }
}

public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
