using System.Net;
using BYDPlatform.Api.Models;
using BYDPlatform.Application.Common.Extensions;
using FluentValidation;

namespace BYDPlatform.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException e)
        {
            var failures = e.Errors;
            await HandleExceptionAsync(context, new Exception(failures.GetValidationErrorMessage()));
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ApplicationException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };
        var responseModel = ApiResponse<string>.Fail(exception.Message);
        await context.Response.WriteAsync(responseModel.ToJsonString());
    }
}