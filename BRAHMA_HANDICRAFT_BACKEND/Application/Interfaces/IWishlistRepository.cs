using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public interface IWishlistRepository
    {
        Task<Guid> AddToWishlistAsync(WishlistItem wishlistItem, CancellationToken cancellationToken);

        Task<List<WishlistItemDto>> GetWishlistedItemsByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        Task<bool> DeleteFromWishlistAsync(Guid userId, Guid wishlistItemId, CancellationToken cancellationToken);
    }
}
