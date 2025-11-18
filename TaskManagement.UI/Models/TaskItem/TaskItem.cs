namespace TaskManagement.UI.Models.TaskItem
{
        public class TaskItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string UserId { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
    }
}
