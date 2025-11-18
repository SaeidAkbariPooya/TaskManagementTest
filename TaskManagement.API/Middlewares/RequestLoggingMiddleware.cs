using System.Diagnostics;
using System.Text;

namespace TaskManagement.API.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly bool _isDevelopment;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _isDevelopment = env.IsDevelopment();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            // لاگ کردن اطلاعات درخواست
            var logContext = new
            {
                Method = request.Method,
                Path = request.Path,
                QueryString = request.QueryString.Value,
                RemoteIp = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = request.Headers["User-Agent"].ToString(),
                ContentType = request.ContentType,
                ContentLength = request.ContentLength
            };

            using (_logger.BeginScope("Request {TraceIdentifier}", context.TraceIdentifier))
            {
                _logger.LogInformation(
                    "HTTP {Method} {Path} started from {RemoteIp}",
                    logContext.Method,
                    logContext.Path,
                    logContext.RemoteIp
                );

                // در محیط توسعه، بدنه درخواست را هم لاگ کنید
                if (_isDevelopment && request.Body.CanRead)
                {
                    await LogRequestBody(context);
                }

                try
                {
                    await _next(context);
                    stopwatch.Stop();

                    var level = context.Response.StatusCode >= 500 ? LogLevel.Error :
                                context.Response.StatusCode >= 400 ? LogLevel.Warning :
                                LogLevel.Information;

                    _logger.Log(level,
                        "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds}ms",
                        request.Method,
                        request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds
                    );
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();

                    _logger.LogError(ex,
                        "HTTP {Method} {Path} failed after {ElapsedMilliseconds}ms",
                        request.Method,
                        request.Path,
                        stopwatch.ElapsedMilliseconds
                    );

                    throw;
                }
            }
        }

        private async Task LogRequestBody(HttpContext context)
        {
            try
            {
                // اجازه می‌دهیم بدنه درخواست مجدداً خوانده شود
                context.Request.EnableBuffering();

                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true
                );

                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrEmpty(body))
                {
                    _logger.LogDebug("Request body: {RequestBody}", body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read request body");
            }
        }
    }
}