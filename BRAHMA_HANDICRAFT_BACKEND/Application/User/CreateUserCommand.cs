using BRAHMA_HANDICRAFT_BACKEND.Domain;
using MediatR;
using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BCrypt.Net;
namespace BRAHMA_HANDICRAFT_BACKEND.Application.User
{
    public record CreateUserCommand(string FirstName, string LastName, string EmailId, string PhoneNumber, bool Role, string Password)
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
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailId = request.EmailId,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            var userId = await _userRepository.AddUserAsync(user);

            return userId;
        }
    }
}
