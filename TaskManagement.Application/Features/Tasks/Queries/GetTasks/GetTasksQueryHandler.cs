using AutoMapper;
using MediatR;
using Serilog;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Core.IRepositories;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasks
{
    public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, Result<IEnumerable<TaskDto>>>
    {
        private readonly ITaskItemRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetTasksQueryHandler(ITaskItemRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<TaskDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
        {
            Log.Information("Getting tasks for user {UserId}", request.UserId);
            var tasks = await _taskRepository.GetByUserIdAsync(request.UserId);

            Log.Debug("Retrieved {TaskCount} tasks for user {UserId}", tasks?.Count() ?? 0, request.UserId);
            var taskDtos = _mapper.Map<List<TaskDto>>(tasks);

            Log.Information("Successfully mapped {TaskCount} tasks to DTOs for user {UserId}",
            taskDtos.Count, request.UserId);
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);
        }
    }
}
