using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StreamNest.Domain.Contracts;
using StreamNest.Domain.Entities.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;



namespace StreamNest.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/problem+json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        var statusCode = contextFeature.Error switch
                        {
                            NotFoundException => StatusCodes.Status404NotFound,
                            BadRequestException => StatusCodes.Status400BadRequest,
                            _ => StatusCodes.Status500InternalServerError
                        };

                        context.Response.StatusCode = statusCode;

                        var problemDetails = new ProblemDetails
                        {
                            Type = "https://httpstatuses.com/" + statusCode,
                            Status = statusCode,
                            Title = GetTitleForStatusCode(statusCode),
                            Detail = contextFeature.Error.Message,
                            Instance = context.Request.Path
                        };

                        logger.LogError($"Exception: {contextFeature.Error.GetType().Name} - {contextFeature.Error.Message}");

                        await context.Response.WriteAsJsonAsync(problemDetails);
                    }
                });
            });
        }

        private static string GetTitleForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status400BadRequest => "Bad Request",
                StatusCodes.Status404NotFound => "Not Found",
                StatusCodes.Status500InternalServerError => "Internal Server Error",
                _ => "An unexpected error occurred"
            };
        }
    }

}