using TaskManagement.UI.Models;
using TaskManagement.UI.Models.Common;
using TaskManagement.UI.Models.TaskItem;

namespace TaskManagement.UI.Services.TaskItem
{
    public interface ITaskService
    {
        Task<ApiResponse<IEnumerable<TaskDto>>> GetAllAsync();
        //Task<ApiResponse<IEnumerable<Models.TaskItem.TaskDto>>> GetAllAsync();
        Task<ApiResponse<Models.TaskItem.TaskDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<Guid>> CreateAsync(CreateTaskRequest request);
        Task<ApiResponse<bool>> UpdateAsync(UpdateTaskRequest request);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
