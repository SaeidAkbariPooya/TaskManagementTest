using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.DTOs.Task;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTaskById
{
    public record GetTaskByIdQuery : IRequest<Result<TaskResponse?>>
    {
        public Guid TaskId { get; init; }
        public Guid UserId { get; set; }
    }
}
