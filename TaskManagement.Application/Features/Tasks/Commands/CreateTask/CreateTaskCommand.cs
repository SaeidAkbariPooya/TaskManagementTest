using MediatR;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Core.Enum;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask
{
    public record CreateTaskCommand : IRequest<TaskResponse>
    {
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public Core.Enum.TaskStatus Status { get; init; }
        public TaskPriority Priority { get; init; }

        public Guid UserId { get; set; }
    }
}
