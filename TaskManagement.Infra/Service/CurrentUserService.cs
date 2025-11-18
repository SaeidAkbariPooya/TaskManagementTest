using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TaskManagement.Core.IServices;

namespace TaskManagement.Infra.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserService> _logger;

        // اضافه کردن logger برای دیباگ
        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Guid? UserId
        {
            get
            {
                try
                {
                    // بررسی وجود HttpContext
                    if (_httpContextAccessor.HttpContext == null)
                    {
                        _logger.LogWarning("HttpContext is null");
                        return null;
                    }

                    // بررسی وجود User
                    if (_httpContextAccessor.HttpContext.User?.Identity == null)
                    {
                        _logger.LogWarning("User or Identity is null");
                        return null;
                    }

                    // بررسی authenticated بودن کاربر
                    if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    {
                        _logger.LogDebug("User is not authenticated");
                        return null;
                    }

                    var userId = _httpContextAccessor.HttpContext.User?
                        .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    // اگر NameIdentifier پیدا نشد، سایر claimها را بررسی کن
                    if (string.IsNullOrEmpty(userId))
                    {
                        userId = _httpContextAccessor.HttpContext.User?
                            .FindFirst("sub")?.Value; // استاندارد JWT
                    }

                    if (string.IsNullOrEmpty(userId))
                    {
                        userId = _httpContextAccessor.HttpContext.User?
                            .FindFirst("uid")?.Value; // claim custom
                    }

                    if (string.IsNullOrEmpty(userId))
                    {
                        _logger.LogWarning("User ID claim not found in: {@Claims}",
                            _httpContextAccessor.HttpContext.User?.Claims.Select(c => new { c.Type, c.Value }));
                        return null;
                    }

                    if (Guid.TryParse(userId, out var result))
                    {
                        return result;
                    }

                    _logger.LogWarning("Failed to parse user ID: {UserId}", userId);
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while getting user ID");
                    return null;
                }
            }
        }

        public string? Email
        {
            get
            {
                try
                {
                    return _httpContextAccessor.HttpContext?.User?
                        .FindFirst(ClaimTypes.Email)?.Value;
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                try
                {
                    return _httpContextAccessor.HttpContext?.User?
                        .Identity?.IsAuthenticated == true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
