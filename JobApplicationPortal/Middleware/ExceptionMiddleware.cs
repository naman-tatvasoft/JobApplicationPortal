using System.Net;
using System.Text.Json;
using JobApplicationPortal.Service.Exceptions;

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
            case RegistrationException _:
                code = HttpStatusCode.BadRequest;
                message = "Invalid registration data.";
                break;
            case EmailAlreadyExistException _:
                code = HttpStatusCode.BadRequest;
                message = "Email already exists.";
                break;
            case LoginException _:
                code = HttpStatusCode.BadRequest;
                message = "Invalid login data.";
                break;
            case EmailDoesNotExistException _:
                code = HttpStatusCode.NotFound;
                message = "Email does not exist.";
                break;
            case InvalidPasswordException _:
                code = HttpStatusCode.Unauthorized;
                message = "Invalid password.";
                break;

            case EmployerNotFoundException _:
                code = HttpStatusCode.NotFound;
                message = "Employer not found.";
                break;
            case JobNameAlreadyExistException _:
                code = HttpStatusCode.BadRequest;
                message = "Job with the same title already exists for this employer.";
                break;
            case SkillNotPresentException _:
                code = HttpStatusCode.NotFound;
                message = "Such skill is not present.";
                break;
            case JobNotFoundException _:
                code = HttpStatusCode.NotFound;
                message = "Job not found.";
                break;
            case JobNotByEmployerException _:
                code = HttpStatusCode.Forbidden;
                message = "Job is not created by the employer.";
                break;
            case JobAlreadyOpenedException _:
                code = HttpStatusCode.Conflict;
                message = "Already opened job cannot be updated.";
                break;
            case JobAlreadyDeleted _:
                code = HttpStatusCode.Conflict;
                message = "Job is already deleted.";
                break;
            
            case ArgumentNullException _:
                code = HttpStatusCode.BadRequest;
                message = "Invalid request. Required data is missing.";
                break;
            case UnauthorizedAccessException _:
                code = HttpStatusCode.Unauthorized;
                message = "You are unauthorized.";
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
            Message = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        
        _logger.LogError($"Status Code: {response.StatusCode}, Message: {response.Message},Exception Details: {exception.Message}");

    }
}
