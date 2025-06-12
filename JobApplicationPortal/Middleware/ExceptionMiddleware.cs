using System.Net;
using System.Text.Json;

namespace JobApplicationPortal.Middleware;

public class ExceptionMiddleware
{

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode code;
        string message;

        switch (exception)
        {
            case ArgumentNullException _:
                code = HttpStatusCode.BadRequest;
                message = "Invalid request. Required data is missing.";
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                message = "Internal server error. Please try again later.";
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = message,
            Details = exception.Message
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        _logger.LogError($"Status Code: {context.Response.StatusCode}, Message: {message}");
        _logger.LogError($"Exception Details: {exception.Message}");

    }


}
