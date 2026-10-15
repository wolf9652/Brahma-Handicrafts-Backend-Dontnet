using BRAHMA_HANDICRAFT_BACKEND.Data;
using Microsoft.AspNetCore.Mvc;
using BRAHMA_HANDICRAFT_BACKEND.Application.User;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
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
        [ProducesResponseType(typeof(SignupResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateUserLogin([FromBody] SignUpRequest request)
        {
            var command = new CreateUserCommand(
                request.FirstName, request.LastName, request.EmailId, request.PhoneNumber, request.Role, request.Password);

            var userId = await _mediator.Send(command);

            return Ok(new SignupResponseDto { UserId = userId, EmailId = request.EmailId, Role = request.Role });
        }

        // POST: api/users/login
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _mediator.Send(new LoginUserCommand(request.EmailId, request.Password));

            return result is null ? Unauthorized() : Ok(result);
        }

        // PUT: api/users/{userId}
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(UpdateUserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateUser(
            Guid userId,
            [FromBody] UpdateUserRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateUserProfileCommand(
                userId, request.FirstName, request.LastName, request.PhoneNumber, request.EmailId, request.Gender);
            var (result, user) = await _mediator.Send(command, cancellationToken);

            return result switch
            {
                UpdateUserResult.NotFound => NotFound(new ErrorResponseDto { Message = "User not found" }),
                UpdateUserResult.EmailConflict => Conflict(new ErrorResponseDto { Message = "Email is already in use by another user" }),
                UpdateUserResult.Success => Ok(new UpdateUserResponseDto
                {
                    UserId = user!.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailId = user.EmailId,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role ? "Customer" : "Admin",
                    Gender = user.Gender?.ToString()
                }),
                _ => StatusCode(500)
            };
        }
    }
}
