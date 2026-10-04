using BRAHMA_HANDICRAFT_BACKEND.Application.Admin;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly IMediator _mediator;

        public AdminUsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/admin/users
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<AdminUserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllUsersQuery(pageNumber, pageSize), cancellationToken);
            return Ok(result);
        }

        // GET: api/admin/users/{userId}
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(AdminUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(userId), cancellationToken);
            return user is null ? NotFound(new { message = "User not found." }) : Ok(user);
        }
    }
}
