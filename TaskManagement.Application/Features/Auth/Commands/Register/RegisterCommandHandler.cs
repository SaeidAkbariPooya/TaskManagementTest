using FluentValidation;
using MediatR;
using Serilog;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Core.Entities;
using TaskManagement.Core.IRepositories;
using TaskManagement.Core.IServices;

namespace TaskManagement.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IValidator<RegisterCommand> _validator;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IValidator<RegisterCommand> validator = null)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _validator = validator;
        }

        public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Registration attempt for user: {Username}", request.FirstName);
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
            //// Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new ValidationException("User with this email already exists");

            Log.Information("Registration failed - user already exists: {Email}", request.Email);

            // Create new user
            var user = new User
            {
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var createdUser = await _userRepository.AddAsync(user);
            var token = _jwtService.GenerateToken(createdUser);

            Log.Information("User registered successfully: {UserId}", createdUser.Id);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                UserId = createdUser.Id,
                Email = createdUser.Email,
                FullName = $"{createdUser.FirstName} {createdUser.LastName}",
                Token = token
            });
        }
    }
}
