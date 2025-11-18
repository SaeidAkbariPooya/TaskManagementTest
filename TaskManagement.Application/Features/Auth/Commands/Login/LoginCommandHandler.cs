using FluentValidation;
using MediatR;
using Serilog;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Core.IRepositories;
using TaskManagement.Core.IServices;

namespace TaskManagement.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IValidator<LoginCommand> _validator;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IValidator<LoginCommand> validator = null)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _validator = validator;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Creating new task for user {UserId}", request.Email);

            if (_validator != null)
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    Log.Warning("Validation failed: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    return null;
                }
            }

            var user = await _userRepository.GetByEmailAsync(request.Email);

            Log.Information("Registration failed - user already exists: {Email}", request.Email);

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password");

            var token = _jwtService.GenerateToken(user);

            Log.Information("User registered successfully: {UserId}", request.Email);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Token = token
            };
        }
    }
}
