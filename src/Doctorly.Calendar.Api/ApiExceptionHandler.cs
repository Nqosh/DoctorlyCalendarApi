using Doctorly.Calendar.Application.Events;
using Doctorly.Calendar.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Doctorly.Calendar.Api;

public partial class ApiExceptionHandler(IProblemDetailsService problemDetails, ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync( HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Not found"),

            ConcurrencyException => (
                StatusCodes.Status409Conflict,
                "Concurrency conflict"),

            DomainException => (
                StatusCodes.Status400BadRequest,
                "Invalid request"),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Unexpected error")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            Unhandled(logger, exception);
        }

        context.Response.StatusCode = status;

        return await problemDetails.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = status == StatusCodes.Status500InternalServerError
                        ? "An unexpected error occurred."
                        : exception.Message
                }
            });
    }

    [LoggerMessage(
        LogLevel.Error,
        "Unhandled API exception")]
    private static partial void Unhandled(
        ILogger logger,
        Exception exception);
}
