using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.DTOs.Responses;
using System.Net;
using System.Text.Json;

namespace CourierManagementSystem.Api.Middleware;

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            NotFoundException notFoundException => new
            {
                statusCode = (int)HttpStatusCode.NotFound,
                body = (object)new ErrorResponse
                {
                    Code = "NOT_FOUND",
                    Message = notFoundException.Message,
                    Timestamp = DateTime.UtcNow
                }
            },
            ValidationException validationException => new
            {
                statusCode = (int)HttpStatusCode.BadRequest,
                body = (object)new ValidationErrorResponse
                {
                    Errors = validationException.Errors
                }
            },
            UnauthorizedException unauthorizedException => new
            {
                statusCode = (int)HttpStatusCode.Unauthorized,
                body = (object)new ErrorResponse
                {
                    Code = "UNAUTHORIZED",
                    Message = unauthorizedException.Message,
                    Timestamp = DateTime.UtcNow
                }
            },
            ForbiddenException forbiddenException => new
            {
                statusCode = (int)HttpStatusCode.Forbidden,
                body = (object)new ErrorResponse
                {
                    Code = "FORBIDDEN",
                    Message = forbiddenException.Message,
                    Timestamp = DateTime.UtcNow
                }
            },
            _ => new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                body = (object)new ErrorResponse
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "An unexpected error occurred",
                    Timestamp = DateTime.UtcNow
                }
            }
        };

        context.Response.StatusCode = response.statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response.body, jsonOptions));
    }
}
