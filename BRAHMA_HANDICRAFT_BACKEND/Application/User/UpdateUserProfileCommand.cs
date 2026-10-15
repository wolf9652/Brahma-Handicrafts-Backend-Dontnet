using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.User
{
    public record UpdateUserProfileCommand(
        Guid UserId,
        string? FirstName,
        string? LastName,
        string? PhoneNumber,
        string? EmailId,
        Gender? Gender) : IRequest<(UpdateUserResult Result, Users? User)>;

    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, (UpdateUserResult Result, Users? User)>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<(UpdateUserResult Result, Users? User)> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            return _userRepository.UpdateUserProfileAsync(
                request.UserId, request.FirstName, request.LastName, request.PhoneNumber, request.EmailId, request.Gender, cancellationToken);
        }
    }
}
