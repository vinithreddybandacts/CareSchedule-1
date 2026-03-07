using System.Net;
using System.Text.Json;
using CareSchedule.API.Contracts;

namespace CareSchedule.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                await WriteResponse(context, HttpStatusCode.NotFound,
                    "RESOURCE_NOT_FOUND", ex.Message);
            }
            catch (ArgumentException ex)
            {
                await WriteResponse(context, HttpStatusCode.BadRequest,
                    "BAD_REQUEST", ex.Message);
            }
            catch (Exception ex)
            {
                var message = _env.IsDevelopment()
                    ? ex.Message
                    : "An unexpected error occurred.";

                await WriteResponse(context, HttpStatusCode.InternalServerError,
                    "INTERNAL_ERROR", message);
            }
        }

        private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode,
            string errorCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.Fail(new { code = errorCode }, message);

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}