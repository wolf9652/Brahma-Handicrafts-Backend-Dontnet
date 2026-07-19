using BRAHMA_HANDICRAFT_BACKEND.Domain;
using MediatR;
using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BCrypt.Net;
namespace BRAHMA_HANDICRAFT_BACKEND.Application.User
{
    public record CreateUserCommand(string Name, string Email, string PhoneNumber, int Role, string Password)
        : IRequest<int>;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new BRAHMA_HANDICRAFT_BACKEND.Domain.Users
            {
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
                Active = true,
                CreatedBy = "System",
                CreatedDTM = DateTime.UtcNow
            };

            var userId = await _userRepository.AddUserAsync(user);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await _userRepository.AddCredentialsAsync(userId, passwordHash);

            return userId;
        }
    }
}
