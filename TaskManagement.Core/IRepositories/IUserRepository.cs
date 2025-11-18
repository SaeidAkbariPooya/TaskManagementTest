using TaskManagement.Core.Entities;

namespace TaskManagement.Core.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User> AddAsync(User user);
    }
}
