using FluentValidation;
using RmConnect.Application.Common.Exceptions;
using RmConnect.Domain.Common;

namespace RmConnect.Api.Errors;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await WriteError(context, StatusCodes.Status400BadRequest, "One or more fields are invalid.",
                ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DomainException ex)
        {
            await WriteError(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteError(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error on {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteError(context, StatusCodes.Status500InternalServerError, "Something went wrong. Please try again.");
        }
    }

    private static Task WriteError(HttpContext context, int status, string message, IEnumerable<string>? errors = null)
    {
        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(new { message, errors });
    }
}
