using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        public readonly IMediator Mediator;
        public BaseController(IMediator mediator)
        {
            Mediator = mediator;
        }
    }
}
