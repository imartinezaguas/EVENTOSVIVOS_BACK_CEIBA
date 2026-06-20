using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using EventosVivos.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace EventosVivos.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            DomainException domainException => ((int)HttpStatusCode.BadRequest, domainException.Message),
            _ => ((int)HttpStatusCode.InternalServerError, "Ocurrió un error inesperado en el servidor.")
        };

        response.StatusCode = statusCode;

        var result = JsonSerializer.Serialize(new { error = message });
        return response.WriteAsync(result);
    }
}
