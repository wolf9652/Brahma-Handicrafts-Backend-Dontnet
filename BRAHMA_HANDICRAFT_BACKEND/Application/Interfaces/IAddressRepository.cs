using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public interface IAddressRepository
    {
        Task<List<AddressDto>> GetAllAddressesByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        Task<Address> AddAddressAsync(Address address, CancellationToken cancellationToken);

        Task<bool> RemoveAddressAsync(Guid userId, Guid addressId, CancellationToken cancellationToken);

        Task<AddressDto?> UpdateAddressAsync(
            Guid userId,
            Guid addressId,
            string customerName,
            string customerPhoneNumber,
            string addressLine1,
            string city,
            string state,
            string postalCode,
            string country,
            bool isDefault,
            CancellationToken cancellationToken);
    }
}
