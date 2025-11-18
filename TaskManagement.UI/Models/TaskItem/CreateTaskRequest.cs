using System.ComponentModel.DataAnnotations;
using TaskManagement.Core.Enum;

namespace TaskManagement.UI.Models.TaskItem
{
    public class CreateTaskRequest
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        public Core.Enum.TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid UserId { get; set; }
    }
}
