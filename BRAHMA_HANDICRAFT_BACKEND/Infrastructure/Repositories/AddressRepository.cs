using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Data;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _context;

        public AddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AddressDto>> GetAllAddressesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Addresses
                .AsNoTracking()
                .Where(a => a.UserId == userId && a.IsActive)
                .OrderBy(a => a.CreatedAt)
                .Select(a => new AddressDto
                {
                    AddressId = a.Id,
                    UserId = a.UserId,
                    CustomerName = a.CustomerName,
                    CustomerPhoneNumber = a.CustomerPhoneNumber,
                    AddressLine1 = a.AddressLine1,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.PostalCode,
                    Country = a.Country,
                    IsDefault = a.IsDefault,
                    IsActive = a.IsActive
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<Address> AddAddressAsync(Address address, CancellationToken cancellationToken)
        {
            // Every newly-added address is active, regardless of what the caller may have set.
            address.IsActive = true;

            // Fall back to the owning user's own name/phone number when not supplied.
            if (string.IsNullOrWhiteSpace(address.CustomerName) || string.IsNullOrWhiteSpace(address.CustomerPhoneNumber))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == address.UserId, cancellationToken);

                if (user is not null)
                {
                    if (string.IsNullOrWhiteSpace(address.CustomerName))
                    {
                        address.CustomerName = $"{user.FirstName} {user.LastName}";
                    }

                    if (string.IsNullOrWhiteSpace(address.CustomerPhoneNumber))
                    {
                        address.CustomerPhoneNumber = user.PhoneNumber;
                    }
                }
            }

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync(cancellationToken);
            return address;
        }

        public async Task<bool> RemoveAddressAsync(Guid userId, Guid addressId, CancellationToken cancellationToken)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(
                a => a.Id == addressId && a.UserId == userId, cancellationToken);

            if (address is null)
            {
                return false;
            }

            if (address.IsDefault)
            {
                var nextDefault = await _context.Addresses
                    .Where(a => a.UserId == userId && a.IsActive && a.Id != addressId)
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                if (nextDefault is not null)
                {
                    nextDefault.IsDefault = true;
                }
            }

            address.IsActive = false;
            address.IsDefault = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<AddressDto?> UpdateAddressAsync(
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
            CancellationToken cancellationToken)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(
                a => a.Id == addressId && a.UserId == userId, cancellationToken);

            if (address is null)
            {
                return null;
            }

            if (isDefault)
            {
                var otherDefaults = await _context.Addresses
                    .Where(a => a.UserId == userId && a.IsActive && a.Id != addressId && a.IsDefault)
                    .ToListAsync(cancellationToken);

                foreach (var other in otherDefaults)
                {
                    other.IsDefault = false;
                }
            }

            address.CustomerName = customerName;
            address.CustomerPhoneNumber = customerPhoneNumber;
            address.AddressLine1 = addressLine1;
            address.City = city;
            address.State = state;
            address.PostalCode = postalCode;
            address.Country = country;
            address.IsDefault = isDefault;

            await _context.SaveChangesAsync(cancellationToken);

            return new AddressDto
            {
                AddressId = address.Id,
                UserId = address.UserId,
                CustomerName = address.CustomerName,
                CustomerPhoneNumber = address.CustomerPhoneNumber,
                AddressLine1 = address.AddressLine1,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                IsDefault = address.IsDefault,
                IsActive = address.IsActive
            };
        }
    }
}
