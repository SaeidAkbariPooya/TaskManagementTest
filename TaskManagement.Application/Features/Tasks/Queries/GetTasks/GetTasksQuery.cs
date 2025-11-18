using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.DTOs.Task;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasks
{
    public record GetTasksQuery : IRequest<Result<IEnumerable<TaskDto>>>
    {
        public Guid UserId { get; set; }
    }
}
