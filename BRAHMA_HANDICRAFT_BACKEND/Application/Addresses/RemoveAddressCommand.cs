using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Addresses
{
    public record RemoveAddressCommand(Guid UserId, Guid AddressId)
        : IRequest<bool>;

    public class RemoveAddressCommandHandler : IRequestHandler<RemoveAddressCommand, bool>
    {
        private readonly IAddressRepository _addressRepository;

        public RemoveAddressCommandHandler(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public Task<bool> Handle(RemoveAddressCommand request, CancellationToken cancellationToken)
        {
            return _addressRepository.RemoveAddressAsync(request.UserId, request.AddressId, cancellationToken);
        }
    }
}
