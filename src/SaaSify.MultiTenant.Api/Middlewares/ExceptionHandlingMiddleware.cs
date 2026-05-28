using System.Net;
using FluentValidation;
using SaaSify.MultiTenant.Shared.Responses;

namespace SaaSify.MultiTenant.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(
                exception,
                exception.Message);

            await HandleExceptionAsync(
                context,
                exception);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType =
            "application/json";

        var response =
            new ApiResponse<object>();

        switch (exception)
        {
            case ValidationException validationException:

                context.Response.StatusCode =
                    (int)HttpStatusCode.BadRequest;

                response.Success = false;

                response.Message = "Validation failed.";

                response.Errors =
                    validationException.Errors
                        .Select(x => x.ErrorMessage)
                        .ToList();

                break;

            case UnauthorizedAccessException:

                context.Response.StatusCode =
                    (int)HttpStatusCode.Unauthorized;

                response.Success = false;

                response.Message = exception.Message;

                break;

            default:

                context.Response.StatusCode =
                    (int)HttpStatusCode.InternalServerError;

                response.Success = false;

                response.Message =
                    "An unexpected error occurred.";

                break;
        }

        await context.Response.WriteAsJsonAsync(response);
    }
}