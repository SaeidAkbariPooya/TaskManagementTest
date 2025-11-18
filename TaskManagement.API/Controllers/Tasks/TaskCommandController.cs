using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTask;
using TaskManagement.Application.Features.Tasks.Queries.GetTaskById;
using TaskManagement.Application.Features.Tasks.Queries.GetTasks;
using TaskManagement.Core.IServices;

namespace TaskManagement.API.Controllers.Tasks
{
    [Route("api/Task")]
    public class TaskCommandController : BaseController
    {
        private readonly ICurrentUserService _currentUserService;
        public TaskCommandController(IMediator mediator, ICurrentUserService currentUserService) : base(mediator)
        {
            _currentUserService = currentUserService;
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(IEnumerable<TaskResponse>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult> GetAll()
        {
            Log.Information("Getting all tasks for user");

            var userId = _currentUserService.UserId!.Value;
            if (!_currentUserService.UserId.HasValue)
                return BadRequest();

            var query = new GetTasksQuery
            {
                UserId = userId
            };

            Log.Information("API: Getting tasks for user {UserId}", userId);
            var tasks = await Mediator.Send(query);

            Log.Information("Successfully retrieved {TaskCount} tasks for user {UserId}", userId);

            return Ok(tasks);
        }

        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(IEnumerable<TaskResponse>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [AllowAnonymous]
        public async Task<ActionResult<TaskResponse>> GetById(Guid id)
        {
            Log.Information("Getting task by ID: {TaskId}", id);

            var userId = _currentUserService.UserId!.Value;
            if (!_currentUserService.UserId.HasValue)
                return BadRequest();

            var query = new GetTaskByIdQuery
            {
                TaskId = id,
                UserId = userId
            };

            Log.Debug("Sending GetTaskByIdQuery for task {TaskId} and user {UserId}", id, userId);
            var task = await Mediator.Send(query);

            if (task == null)
            {
                Log.Warning("Task with ID {TaskId} not found", id);
                return NotFound();
            }

            Log.Information("Successfully retrieved task {TaskId}", id);
            return Ok(task);
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(TaskResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<TaskResponse>> Create(CreateTaskCommand command)
        {
            Log.Information("Creating new task");

            var userId = _currentUserService.UserId!.Value;
            if (!_currentUserService.UserId.HasValue)
                return BadRequest();

            command.UserId = userId;


            Log.Information("API: Creating task for user {UserId}", command.UserId);

            var task = await Mediator.Send(command);

            Log.Information("Successfully created task with ID {TaskId}", task.Id);

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("Update")]
        [ProducesResponseType(typeof(TaskResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Update(UpdateTaskCommand command)
        {
            Log.Information("Updating task with ID: {TaskId}", command.TaskId);

            var userId = _currentUserService.UserId!.Value;
            if (!_currentUserService.UserId.HasValue)
                return BadRequest();

            command.UserId = userId;

            Log.Debug("Sending UpdateTaskCommand for task {TaskId} and user {UserId}", command.TaskId, command.UserId);
            var task = await Mediator.Send(command);

            if (task == null)
            {
                Log.Warning("Task with ID {TaskId} not found for update", command.TaskId);
                return NotFound();
            }

            Log.Information("Successfully updated task with ID {TaskId}", command.TaskId);
            return Ok(task);
        }

        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            Log.Information("Deleting task with ID: {TaskId}", id);

            var userId = _currentUserService.UserId!.Value;

            if (!_currentUserService.UserId.HasValue)
                return BadRequest();

            var command = new DeleteTaskCommand
            {
                TaskId = id,
                UserId = userId
            };

            Log.Debug("Sending DeleteTaskCommand for task {TaskId} and user {UserId}", id, command.UserId);
            var result = await Mediator.Send(command);

            if (!result)
            {
                Log.Warning("Task with ID {TaskId} not found for deletion", id);
                return NotFound();
            }

            Log.Information("Successfully deleted task with ID {TaskId}", id);
            return NoContent();
        }
    }
}
