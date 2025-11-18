using AutoMapper;
using FluentValidation;
using MediatR;
using Serilog;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Core.IRepositories;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskResponse?>
    {
        private readonly ITaskItemRepository _taskRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateTaskCommand> _validator;
        public UpdateTaskCommandHandler(ITaskItemRepository taskRepository, IMapper mapper = null, IValidator<UpdateTaskCommand> validator = null)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<TaskResponse?> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Updating task {TaskId} for user {UserId}",
                             request.TaskId, request.UserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                Log.Warning("❌ Validation failed for UpdateTaskCommand - TaskId: {TaskId}, UserId: {UserId}, Errors: {Errors}",
                    request.TaskId, request.UserId,
                    string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));
                return null;
            }

            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            if (task == null || task.UserId != request.UserId)
                return null;

            if (task == null || task.UserId != request.UserId)
            {
                Log.Warning("Task not found or access denied for task {TaskId}", request.TaskId);
                return null;
            }

            // اعمال تغییرات
            if (!string.IsNullOrEmpty(request.Title))
                task.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Description))
                task.Description = request.Description;

            if (request.Status.HasValue)
                task.Status = request.Status.Value;

            if (request.Priority.HasValue)
                task.Priority = request.Priority.Value;

            await _taskRepository.UpdateAsync(task);

            var response = _mapper.Map<TaskResponse>(task);

            Log.Information("Task {TaskId} updated successfully", request.TaskId);

            return response;
        }
    }
}
