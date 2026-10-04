using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record GetUserByIdQuery(Guid UserId) : IRequest<AdminUserDto?>;

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, AdminUserDto?>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<AdminUserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        }
    }
}
