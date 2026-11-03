using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using TSP.Mediator.Dto.Response;

namespace TSP.Api.Middleware
{
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
                _logger.LogError(ex, "An exception has occurred.");

                if (context.Response.HasStarted) throw;

                var response = HandleException(ex);

                context.Response.Clear();
                context.Response.StatusCode = response.StatusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(response.Body), context.RequestAborted);
            }
        }

        private (int StatusCode, BaseResponse Body) HandleException(Exception ex)
        {
            return ex switch
            {
                ValidationException => (400, new BaseResponse()
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = new List<string> { ex.Message }
                }),

                UnauthorizedAccessException => (401, new BaseResponse
                {
                    Success = false,
                    Message = "Unauthorized.",
                    Errors = new List<string> { ex.Message }
                }),

                _ => (500, new BaseResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred.",
                    Errors = null
                })
            };
        }
    }
}
