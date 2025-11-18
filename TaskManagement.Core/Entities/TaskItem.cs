using TaskManagement.Core.Enum;

namespace TaskManagement.Core.Entities
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public Enum.TaskStatus Status { get; set; } = Enum.TaskStatus.Todo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
    }
}
