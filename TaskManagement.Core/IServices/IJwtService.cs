using TaskManagement.Core.Entities;

namespace TaskManagement.Core.IServices
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
