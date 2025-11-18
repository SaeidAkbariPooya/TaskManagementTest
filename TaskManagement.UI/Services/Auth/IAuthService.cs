using TaskManagement.UI.Models.Auth;
using TaskManagement.UI.Models.Common;

namespace TaskManagement.UI.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponse>> LoginAsync(Models.Auth.LoginRequest request);
        Task<ApiResponse<AuthResponse>> RegisterAsync(Models.Auth.RegisterRequest request);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task<Guid?> GetUserIdAsync();
        Task<string> GetFullNameAsync();
    }
}
