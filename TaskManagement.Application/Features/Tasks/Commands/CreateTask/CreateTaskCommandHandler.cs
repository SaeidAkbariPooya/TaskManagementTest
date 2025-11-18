using AutoMapper;
using FluentValidation;
using MediatR;
using Serilog;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Core.Entities;
using TaskManagement.Core.IRepositories;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskResponse>
    {
        private readonly ITaskItemRepository _taskRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateTaskCommand> _validator;
        public CreateTaskCommandHandler(ITaskItemRepository taskRepository, IMapper mapper = null, IValidator<CreateTaskCommand> validator = null)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<TaskResponse> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Creating new task for user {UserId}", request.UserId);
            if (_validator != null)
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    Log.Warning("Validation failed: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    return null;
                }
            }

            var task = _mapper.Map<TaskItem>(request);
            var createdTask = await _taskRepository.AddAsync(task);

            Log.Information("Task created successfully - ID: {TaskId}", createdTask.Id);
            return _mapper.Map<TaskResponse>(createdTask);
        }
    }
}
