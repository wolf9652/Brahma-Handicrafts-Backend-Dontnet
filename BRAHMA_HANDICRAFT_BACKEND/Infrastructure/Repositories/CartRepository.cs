using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Data;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddToCartAsync(
            Guid userId,
            Guid productId,
            Guid designId,
            Guid sizeId,
            int quantity,
            CancellationToken cancellationToken)
        {
            var existing = await _context.CartItems.FirstOrDefaultAsync(
                ci => ci.UserId == userId && ci.ProductId == productId && ci.DesignId == designId && ci.SizeId == sizeId,
                cancellationToken);

            if (existing is not null)
            {
                existing.Quantity += quantity;
                await _context.SaveChangesAsync(cancellationToken);
                return existing.Id;
            }

            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                DesignId = designId,
                SizeId = sizeId,
                Quantity = quantity
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync(cancellationToken);
            return cartItem.Id;
        }

        public async Task<UpdateCartItemQuantityResult> UpdateCartItemQuantityAsync(
            Guid userId,
            Guid cartItemId,
            int quantity,
            CancellationToken cancellationToken)
        {
            if (quantity < 0)
            {
                return UpdateCartItemQuantityResult.InvalidQuantity;
            }

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(
                ci => ci.Id == cartItemId && ci.UserId == userId, cancellationToken);

            if (cartItem is null)
            {
                return UpdateCartItemQuantityResult.NotFound;
            }

            if (quantity == 0)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync(cancellationToken);
                return UpdateCartItemQuantityResult.Deleted;
            }

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync(cancellationToken);
            return UpdateCartItemQuantityResult.Updated;
        }

        public async Task<bool> DeleteCartItemAsync(Guid userId, Guid cartItemId, CancellationToken cancellationToken)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(
                ci => ci.Id == cartItemId && ci.UserId == userId, cancellationToken);

            if (cartItem is null)
            {
                return false;
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task ClearCartAsync(Guid userId, CancellationToken cancellationToken)
        {
            await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<List<CartItemDto>> GetCartItemsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.CartItems
                .AsNoTracking()
                .Where(ci => ci.UserId == userId && ci.DesignId != null && ci.SizeId != null && ci.Product.IsActive)
                .OrderBy(ci => ci.CreatedAt)
                .Select(ci => new CartItemDto
                {
                    CartItemId = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    DesignId = ci.DesignId!.Value,
                    SizeId = ci.SizeId!.Value,
                    SizeName = ci.Size!.SizeName,
                    BasePrice = ci.Size.BasePrice,
                    Quantity = ci.Quantity,
                    Design = new DesignSummaryDto
                    {
                        DesignId = ci.Design!.Id,
                        DesignName = ci.Design.DesignName,
                        DesignDescription = ci.Design.DesignDescription,
                        EstimatedPrice = ci.Design.EstimatedPrice,
                        TotalQuantity = ci.Design.InventoryStocks.Sum(i => (int?)i.Quantity) ?? 0,
                        // Count of Sizes belonging to this Design.
                        TotalSizes = ci.Design.Sizes.Count(),
                        // Prefer the primary-flagged image; among duplicates or when none is primary,
                        // fall back deterministically to the lowest Id.
                        Image = ci.Design.ProductImages
                            .OrderByDescending(pi => pi.IsPrimary)
                            .ThenBy(pi => pi.Id)
                            .Select(pi => new ProductImageDto
                            {
                                ImageId = pi.Id,
                                Url = pi.ImageURL,
                                AltText = pi.AltText,
                                IsPrimary = pi.IsPrimary
                            })
                            .FirstOrDefault()
                    }
                })
                .ToListAsync(cancellationToken);
        }
    }
}
