using BRAHMA_HANDICRAFT_BACKEND.Domain;
using MediatR;
using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BCrypt.Net;
namespace BRAHMA_HANDICRAFT_BACKEND.Application.User
{
    public record CreateUserCommand(string Name, string Email, string PhoneNumber, int Role, string Password)
        : IRequest<Guid>;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new BRAHMA_HANDICRAFT_BACKEND.Domain.Users
            {
                FirstName = request.Name,
                LastName = string.Empty,
                EmailId = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role == 2, // old contract: 1 = Admin, 2 = Customer -> new bool, true = Customer
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            var userId = await _userRepository.AddUserAsync(user);

            return userId;
        }
    }
}
