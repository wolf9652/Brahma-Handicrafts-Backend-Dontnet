using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Addresses
{
    public record UpdateAddressCommand(
        Guid UserId,
        Guid AddressId,
        string CustomerName,
        string CustomerPhoneNumber,
        string AddressLine1,
        string City,
        string State,
        string PostalCode,
        string Country,
        bool IsDefault) : IRequest<AddressDto?>;

    public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, AddressDto?>
    {
        private readonly IAddressRepository _addressRepository;

        public UpdateAddressCommandHandler(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public Task<AddressDto?> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            return _addressRepository.UpdateAddressAsync(
                request.UserId,
                request.AddressId,
                request.CustomerName,
                request.CustomerPhoneNumber,
                request.AddressLine1,
                request.City,
                request.State,
                request.PostalCode,
                request.Country,
                request.IsDefault,
                cancellationToken);
        }
    }
}
