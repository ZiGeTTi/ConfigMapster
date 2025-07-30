using ConfigMapster.API.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConfigMapster.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception occurred.");

                var problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "An unexpected error occurred.",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = problemDetails.Status.Value;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
            }
        }
        private static (int, string) GetStatusCode(Exception exception) => exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, nameof(StatusCodes.Status400BadRequest)),

            ObjectNotFoundException => (StatusCodes.Status404NotFound, nameof(StatusCodes.Status404NotFound)),

            RecordAlreadyExistException => (StatusCodes.Status409Conflict, nameof(StatusCodes.Status409Conflict)),

            _ => (StatusCodes.Status500InternalServerError, nameof(StatusCodes.Status500InternalServerError))
        };
    }
}
