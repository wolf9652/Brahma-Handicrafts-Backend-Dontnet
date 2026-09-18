using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Products;
using BRAHMA_HANDICRAFT_BACKEND.Data;
using Microsoft.EntityFrameworkCore;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<ProductProjection> Items, int TotalCount)> GetProductsAsync(
            bool includeInactive,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(p => p.IsActive);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(p => p.CreatedAt).ThenBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductProjection
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    DefaultPrice = p.DefaultPrice,
                    DefaultMRP = p.DefaultMRP,
                    IsActive = p.IsActive,
                    Designs = p.Designs.Select(d => new DesignProjection
                    {
                        Id = d.Id,
                        DesignName = d.DesignName,
                        DesignDescription = d.DesignDescription,
                        Sizes = d.Sizes.Select(s => new SizeProjection
                        {
                            Id = s.Id,
                            SizeName = s.SizeName,
                            Length = s.Length,
                            Breadth = s.Breadth,
                            Height = s.Height,
                            Weight = s.Weight,
                            BasePrice = s.BasePrice,
                            MRP = s.MRP,
                            InventoryStockId = s.InventoryStocks.Select(i => (Guid?)i.Id).FirstOrDefault(),
                            Quantity = s.InventoryStocks.Select(i => (int?)i.Quantity).FirstOrDefault()
                        }).ToList(),
                        Images = d.ProductImages.Select(pi => new ImageProjection
                        {
                            Id = pi.Id,
                            Url = pi.ImageURL,
                            AltText = pi.AltText,
                            IsPrimary = pi.IsPrimary
                        }).ToList()
                    }).ToList(),
                    ProductSizes = p.Sizes.Where(s => s.DesignId == null).Select(s => new SizeProjection
                    {
                        Id = s.Id,
                        SizeName = s.SizeName,
                        Length = s.Length,
                        Breadth = s.Breadth,
                        Height = s.Height,
                        Weight = s.Weight,
                        BasePrice = s.BasePrice,
                        MRP = s.MRP,
                        InventoryStockId = s.InventoryStocks.Select(i => (Guid?)i.Id).FirstOrDefault(),
                        Quantity = s.InventoryStocks.Select(i => (int?)i.Quantity).FirstOrDefault()
                    }).ToList(),
                    ProductImages = p.ProductImages.Where(pi => pi.DesignId == null).Select(pi => new ImageProjection
                    {
                        Id = pi.Id,
                        Url = pi.ImageURL,
                        AltText = pi.AltText,
                        IsPrimary = pi.IsPrimary
                    }).ToList(),
                    ProductInventory = p.InventoryStocks
                        .Where(i => i.SizeId == null && i.DesignId == null)
                        .Select(i => new InventoryProjection { Id = i.Id, Quantity = i.Quantity })
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
