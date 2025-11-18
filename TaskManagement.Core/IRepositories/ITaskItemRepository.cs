
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.IRepositories
{
    public interface ITaskItemRepository
    {
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<IEnumerable<TaskItem>> GetByUserIdAsync(Guid userId);
        Task<TaskItem> AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(Guid id);
    }
}
