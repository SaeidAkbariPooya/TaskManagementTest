using Microsoft.AspNetCore.Identity;
using TaskManagement.Core.IServices;

namespace TaskManagement.Infra.Service
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _passwordHasher = new();

        public string HashPassword(string password)
            => _passwordHasher.HashPassword(null!, password);

        public bool VerifyPassword(string password, string passwordHash)
            => _passwordHasher.VerifyHashedPassword(null!, passwordHash, password)
                == PasswordVerificationResult.Success;
    }
}
