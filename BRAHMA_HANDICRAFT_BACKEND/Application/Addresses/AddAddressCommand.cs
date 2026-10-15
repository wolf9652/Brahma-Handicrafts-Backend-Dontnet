using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Addresses
{
    public record AddAddressCommand(
        Guid UserId,
        string? CustomerName,
        string? CustomerPhoneNumber,
        string AddressLine1,
        string City,
        string State,
        string PostalCode,
        string Country,
        bool IsDefault) : IRequest<Address>;

    public class AddAddressCommandHandler : IRequestHandler<AddAddressCommand, Address>
    {
        private readonly IAddressRepository _addressRepository;

        public AddAddressCommandHandler(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public Task<Address> Handle(AddAddressCommand request, CancellationToken cancellationToken)
        {
            var address = new Address
            {
                UserId = request.UserId,
                CustomerName = request.CustomerName ?? string.Empty,
                CustomerPhoneNumber = request.CustomerPhoneNumber ?? string.Empty,
                AddressLine1 = request.AddressLine1,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                IsDefault = request.IsDefault,
                IsActive = true
            };

            return _addressRepository.AddAddressAsync(address, cancellationToken);
        }
    }
}
