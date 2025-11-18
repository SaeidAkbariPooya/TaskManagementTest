using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel.DataAnnotations;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.Features.Auth.Commands.Login;
using TaskManagement.Application.Features.Auth.Commands.Register;

namespace TaskManagement.API.Controllers.Auth
{
    [Route("api/Auth")]
    public class AuthCommandController : BaseController
    {
        public AuthCommandController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterCommand command)
        {
            try
            {
                Log.Information("API: Registration attempt for {Email}", command.Email);
                var result = await Mediator.Send(command);

                Log.Information("API: Registration successful for {Email}",
                    command.Email);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                Log.Warning("API: Registration validation failed for {Email} - Error: {ErrorMessage}",
                    command.Email, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "API: Registration system error for {Email}", command.Email);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginCommand query)
        {
            try
            {
                Log.Information("API: Login attempt for {Email}", query.Email);
                var result = await Mediator.Send(query);

                Log.Information("API: Login successful for {Email} - User ID: {UserId}",
                    query.Email, result.UserId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Warning("API: Login unauthorized for {Email} - Reason: {ErrorMessage}",
                    query.Email, ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "API: Login system error for {Email}", query.Email);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
