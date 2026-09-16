using BRAHMA_HANDICRAFT_BACKEND.Data;
using Microsoft.AspNetCore.Mvc;
using BRAHMA_HANDICRAFT_BACKEND.Application.User;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;

        public UsersController(AppDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        // POST: api/users/signup
        [HttpPost("signup")]
        public async Task<IActionResult> CreateUserLogin([FromBody] SignUpRequest request)
        {
            var command = new CreateUserCommand(request.Name, request.Email, request.PhoneNumber, request.Role, request.Password);

            var userId = await _mediator.Send(command);

            return Ok(new { UserId = userId, Email = request.Email, Role = request.Role });
        }
    }
}
