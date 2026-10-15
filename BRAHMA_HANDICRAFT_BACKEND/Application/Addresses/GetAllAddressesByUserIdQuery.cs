using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Addresses
{
    public record GetAllAddressesByUserIdQuery(Guid UserId)
        : IRequest<List<AddressDto>>;

    public class GetAllAddressesByUserIdQueryHandler : IRequestHandler<GetAllAddressesByUserIdQuery, List<AddressDto>>
    {
        private readonly IAddressRepository _addressRepository;

        public GetAllAddressesByUserIdQueryHandler(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public Task<List<AddressDto>> Handle(GetAllAddressesByUserIdQuery request, CancellationToken cancellationToken)
        {
            return _addressRepository.GetAllAddressesByUserIdAsync(request.UserId, cancellationToken);
        }
    }
}
