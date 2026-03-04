using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AiGateway.WebApi.Exceptions;

public class GlobalExceptionHandler: IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Sistemde bir hata yakalandı: {Message}", exception.Message);
        if (exception is ValidationException validationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Doğrulama Hatası (Validation Error)",
                Detail = "Gönderilen istek kurallara uymuyor. Lütfen hataları kontrol edin.",
                Type = "https://datatracker.ietf.org/doc/html/rfc7807"
            };
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());
            problemDetails.Extensions.Add("errors", errors);

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var defaultProblemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Sunucu Hatası",
            Detail = "İşlem sırasında beklenmeyen bir hata oluştu."
        };

        await httpContext.Response.WriteAsJsonAsync(defaultProblemDetails, cancellationToken);
        return true;
    }
}