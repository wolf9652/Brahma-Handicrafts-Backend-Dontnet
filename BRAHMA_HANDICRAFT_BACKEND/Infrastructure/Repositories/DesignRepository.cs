using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Data;
using Microsoft.EntityFrameworkCore;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Repositories
{
    public class DesignRepository : IDesignRepository
    {
        private readonly AppDbContext _context;

        public DesignRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DesignDetailsDto?> GetDesignDetailsByIdAsync(Guid productId, Guid designId, CancellationToken cancellationToken)
        {
            return await _context.Designs
                .AsNoTracking()
                .Where(d => d.Id == designId && d.ProductId == productId)
                .Select(d => new DesignDetailsDto
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product.Name,
                    DesignId = d.Id,
                    DesignName = d.DesignName,
                    DesignDescription = d.DesignDescription,
                    EstimatedPrice = d.EstimatedPrice,
                    Images = d.ProductImages.Select(pi => new ProductImageDto
                    {
                        ImageId = pi.Id,
                        Url = pi.ImageURL,
                        AltText = pi.AltText,
                        IsPrimary = pi.IsPrimary
                    }).ToList(),
                    Sizes = d.Sizes.Select(s => new SizeWithInventoryDto
                    {
                        SizeId = s.Id,
                        SizeName = s.SizeName,
                        Length = s.Length,
                        Breadth = s.Breadth,
                        Height = s.Height,
                        Weight = s.Weight,
                        BasePrice = s.BasePrice,
                        MRP = s.MRP,
                        InventoryStockId = s.InventoryStocks.Select(i => (Guid?)i.Id).FirstOrDefault(),
                        Quantity = s.InventoryStocks.Select(i => (int?)i.Quantity).FirstOrDefault()
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
