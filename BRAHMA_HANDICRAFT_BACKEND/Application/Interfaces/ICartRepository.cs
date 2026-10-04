using BRAHMA_HANDICRAFT_BACKEND.Application.Models;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public enum UpdateCartItemQuantityResult
    {
        NotFound,
        InvalidQuantity,
        Deleted,
        Updated
    }

    public interface ICartRepository
    {
        Task<Guid> AddToCartAsync(
            Guid userId,
            Guid productId,
            Guid designId,
            Guid sizeId,
            int quantity,
            CancellationToken cancellationToken);

        Task<List<CartItemDto>> GetCartItemsByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        Task<UpdateCartItemQuantityResult> UpdateCartItemQuantityAsync(
            Guid userId,
            Guid cartItemId,
            int quantity,
            CancellationToken cancellationToken);

        Task<bool> DeleteCartItemAsync(Guid userId, Guid cartItemId, CancellationToken cancellationToken);
    }
}
