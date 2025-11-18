using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.DTOs.Auth;

namespace TaskManagement.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
    }
}
