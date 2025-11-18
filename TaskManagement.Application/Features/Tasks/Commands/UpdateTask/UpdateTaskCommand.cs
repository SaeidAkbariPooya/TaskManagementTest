using MediatR;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Core.Enum;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask
{
    public record UpdateTaskCommand : IRequest<TaskResponse?>
    {
        public Guid TaskId { get; init; }
        public Guid UserId { get; set; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public Core.Enum.TaskStatus? Status { get; init; }
        public TaskPriority? Priority { get; init; }
    }
}
