using FluentValidation;
using MediatR;
using Serilog;
using TaskManagement.Core.IRepositories;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
    {
        private readonly ITaskItemRepository _taskRepository;
        private readonly IValidator<DeleteTaskCommand> _validator;

        public DeleteTaskCommandHandler(ITaskItemRepository taskRepository, IValidator<DeleteTaskCommand> validator)
        {
            _taskRepository = taskRepository;
            _validator = validator;
        }

        public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Deleting task {TaskId} for user {UserId}",
                             request.TaskId, request.UserId);

            // اعتبارسنجی - روش مستقیم
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                Log.Warning("❌ Validation failed for DeleteTaskCommand - TaskId: {TaskId}, UserId: {UserId}, Errors: {Errors}",
                                       request.TaskId, request.UserId,
                                       string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));
                return false;
            }

            var task = await _taskRepository.GetByIdAsync(request.TaskId);

            if (task == null)
            {
                Log.Warning("Task {TaskId} not found for deletion", request.TaskId);
                return false;
            }

            if (task.UserId != request.UserId)
            {
                Log.Warning("User {UserId} not authorized to delete task {TaskId}",
                    request.UserId, request.TaskId);
                return false;
            }

            await _taskRepository.DeleteAsync(request.TaskId);
            Log.Information("Task {TaskId} deleted successfully by user {UserId}",
                            request.TaskId, request.UserId);

            return true;
        }
    }
}
