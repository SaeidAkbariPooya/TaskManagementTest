using MediatR;
using TaskManagement.Application.DTOs.Auth;


namespace TaskManagement.Application.Features.Auth.Commands.Login
{
    public record LoginCommand : IRequest<AuthResponseDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
