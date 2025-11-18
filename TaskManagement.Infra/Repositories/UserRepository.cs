using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Core.IRepositories;
using TaskManagement.Infra.Context;

namespace TaskManagement.Infra.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskManagmentDbContext _context;

        public UserRepository(TaskManagmentDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                        .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
