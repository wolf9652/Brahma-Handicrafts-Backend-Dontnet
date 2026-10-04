using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Data;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext _context;

        public WishlistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddToWishlistAsync(WishlistItem wishlistItem, CancellationToken cancellationToken)
        {
            _context.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync(cancellationToken);
            return wishlistItem.Id;
        }

        public async Task<bool> DeleteFromWishlistAsync(Guid userId, Guid wishlistItemId, CancellationToken cancellationToken)
        {
            var wishlistItem = await _context.WishlistItems.FirstOrDefaultAsync(
                wi => wi.Id == wishlistItemId && wi.UserId == userId, cancellationToken);

            if (wishlistItem is null)
            {
                return false;
            }

            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<WishlistItemDto>> GetWishlistedItemsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.WishlistItems
                .AsNoTracking()
                .Where(wi => wi.UserId == userId && wi.DesignId != null && wi.Product.IsActive)
                .OrderBy(wi => wi.CreatedAt)
                .Select(wi => new WishlistItemDto
                {
                    WishlistItemId = wi.Id,
                    ProductId = wi.ProductId,
                    ProductName = wi.Product.Name,
                    IsActive = wi.Product.IsActive,
                    Design = new DesignSummaryDto
                    {
                        DesignId = wi.Design!.Id,
                        DesignName = wi.Design.DesignName,
                        DesignDescription = wi.Design.DesignDescription,
                        EstimatedPrice = wi.Design.EstimatedPrice,
                        TotalQuantity = wi.Design.InventoryStocks.Sum(i => (int?)i.Quantity) ?? 0,
                        // Count of Sizes belonging to this Design.
                        TotalSizes = wi.Design.Sizes.Count(),
                        // Prefer the primary-flagged image; among duplicates or when none is primary,
                        // fall back deterministically to the lowest Id.
                        Image = wi.Design.ProductImages
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
