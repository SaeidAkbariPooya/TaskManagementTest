
namespace TaskManagement.Application.DTOs.Auth
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
