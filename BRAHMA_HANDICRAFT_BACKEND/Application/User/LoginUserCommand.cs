using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BCrypt.Net;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.User
{
    public record LoginUserCommand(string EmailId, string Password)
        : IRequest<LoginResponseDto?>;

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto?>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginUserCommandHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponseDto?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.EmailId);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailId = user.EmailId,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role ? "Customer" : "Admin",
                Gender = user.Gender?.ToString()
            };
        }
    }
}
