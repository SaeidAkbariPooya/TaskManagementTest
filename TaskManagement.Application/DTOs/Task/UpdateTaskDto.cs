using System.ComponentModel.DataAnnotations;
using TaskManagement.Core.Enum;

namespace TaskManagementTest.Application.DTOs.Task
{
    public class UpdateTaskDto
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public TaskManagement.Core.Enum.TaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
    }
}
