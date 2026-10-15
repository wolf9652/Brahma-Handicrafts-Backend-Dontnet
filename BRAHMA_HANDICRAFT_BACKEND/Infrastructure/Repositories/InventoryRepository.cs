using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Data;
using Microsoft.EntityFrameworkCore;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<InventoryItemDto> Items, int TotalCount)> GetAllInventoryAsync(
            bool includeInactive,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.InventoryStocks
                .AsNoTracking()
                .Where(i => includeInactive || i.Product.IsActive)
                .OrderBy(i => i.Product.Name)
                .ThenBy(i => i.Design.DesignName)
                .ThenBy(i => i.Size.SizeName);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new InventoryItemDto
                {
                    InventoryStockId = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    DesignId = i.DesignId,
                    DesignName = i.Design.DesignName,
                    SizeId = i.SizeId,
                    SizeName = i.Size.SizeName,
                    Quantity = i.Quantity,
                    Image = i.Design.ProductImages
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
                })
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<InventoryItemDto?> UpdateQuantityAsync(
            Guid inventoryStockId,
            int quantity,
            CancellationToken cancellationToken)
        {
            var inventoryStock = await _context.InventoryStocks
                .FirstOrDefaultAsync(i => i.Id == inventoryStockId, cancellationToken);

            if (inventoryStock is null)
            {
                return null;
            }

            inventoryStock.Quantity = quantity;
            inventoryStock.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return await _context.InventoryStocks
                .AsNoTracking()
                .Where(i => i.Id == inventoryStockId)
                .Select(i => new InventoryItemDto
                {
                    InventoryStockId = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    DesignId = i.DesignId,
                    DesignName = i.Design.DesignName,
                    SizeId = i.SizeId,
                    SizeName = i.Size.SizeName,
                    Quantity = i.Quantity,
                    Image = i.Design.ProductImages
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
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
