using MediatR;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask
{
    public record DeleteTaskCommand : IRequest<bool>
    {
        public Guid TaskId { get; init; }
        public Guid UserId { get; set; }
    }
}
