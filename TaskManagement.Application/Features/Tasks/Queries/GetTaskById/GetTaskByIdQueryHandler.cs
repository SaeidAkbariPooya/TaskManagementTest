using AutoMapper;
using MediatR;
using Serilog;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Core.IRepositories;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTaskById
{
    internal class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskResponse?>>
    {
        private readonly ITaskItemRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetTaskByIdQueryHandler(ITaskItemRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<Result<TaskResponse?>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            Log.Information("Getting task by ID: {TaskId} for user {UserId}",
                            request.TaskId, request.UserId);
            var task = await _taskRepository.GetByIdAsync(request.TaskId);

            if (task == null)
            {
                Log.Warning("Task with ID {TaskId} not found for user {UserId}",
                    request.TaskId, request.UserId);
                return Result<TaskResponse?>.Success(null);
            }

            if (task.UserId != request.UserId)
            {
                Log.Warning("User {UserId} attempted to access task {TaskId} owned by user {TaskOwnerId}",
                    request.UserId, request.TaskId, task.UserId);
                return Result<TaskResponse?>.Success(null);
            }

            var taskDtos = _mapper.Map<TaskResponse>(task);
            Log.Information("Successfully retrieved task {TaskId} for user {UserId}",
                request.TaskId, request.UserId);

            Log.Debug("Task details - Title: {TaskTitle}, Priority: {TaskPriority}, Status: {TaskStatus}",
                task.Title, task.Priority, task.Status);

            return Result<TaskResponse?>.Success(taskDtos);
        }
    }
}
