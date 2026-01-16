using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace smartMonitoringBE.Middleware;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _log;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> log)
    {
        _log = log;
    }

    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (OperationCanceledException) when (ctx.RequestAborted.IsCancellationRequested)
        {
            // Client disconnected / cancelled request. Not an error.
            ctx.Response.StatusCode = 499; // non-standard but common (nginx style)
        }
        catch (Exception ex)
        {
            if (ctx.Response.HasStarted)
            {
                // Too late to write a clean response
                throw;
            }

            var traceId = ctx.TraceIdentifier;
            var oid = ctx.User?.FindFirst("oid")?.Value;
            var tid = ctx.User?.FindFirst("tid")?.Value;

            var (status, title, safeDetail, logLevel) = Map(ex);

            // Log with appropriate severity
            _log.Log(logLevel, ex,
                "Request failed. TraceId={TraceId} Status={Status} Method={Method} Path={Path} Oid={Oid} Tid={Tid}",
                traceId, status, ctx.Request.Method, ctx.Request.Path, oid, tid);

            await WriteProblemDetailsAsync(ctx, status, title, safeDetail, traceId);
        }
    }

    private static (int status, string title, string detail, LogLevel logLevel) Map(Exception ex)
    {
        return ex switch
        {
            // Your domain exceptions
            AppException appEx => (
                appEx.StatusCode,
                appEx.Title,
                appEx.Message,          // ok to expose for known/expected cases
                LogLevel.Warning        // expected → warning
            ),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorised",
                "Authentication required.",
                LogLevel.Warning
            ),

          

            // Everything else → 500
            _ => (
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.",
                LogLevel.Error
            )
        };
    }

    private static Task WriteProblemDetailsAsync(
        HttpContext ctx,
        int status,
        string title,
        string detail,
        string traceId)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = ctx.Request.Path
        };

        problem.Extensions["traceId"] = traceId;

        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = status;

        return ctx.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOpts));
    }
}