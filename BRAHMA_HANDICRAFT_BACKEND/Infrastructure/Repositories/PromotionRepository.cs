using BRAHMA_HANDICRAFT_BACKEND.Application.Admin;
using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Data;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly AppDbContext _context;

        public PromotionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(Promotion Promotion, int ProductCount)> CreatePromotionAsync(
            Promotion promotion,
            List<Guid> productIds,
            CancellationToken cancellationToken)
        {
            _context.Promotions.Add(promotion);

            var validProductIds = new List<Guid>();

            if (productIds.Count > 0)
            {
                validProductIds = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync(cancellationToken);

                foreach (var productId in validProductIds)
                {
                    _context.PromotionProducts.Add(new PromotionProduct
                    {
                        PromotionId = promotion.Id,
                        ProductId = productId
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return (promotion, validProductIds.Count);
        }

        public async Task<(List<(Promotion Promotion, int ProductCount, int BannerCount)> Items, int TotalCount)> GetAllPromotionsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Promotions.AsNoTracking().OrderByDescending(p => p.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    Promotion = p,
                    ProductCount = p.PromotionProducts.Count(),
                    BannerCount = p.PromotionBanners.Count()
                })
                .ToListAsync(cancellationToken);

            var result = items.Select(i => (i.Promotion, i.ProductCount, i.BannerCount)).ToList();

            return (result, totalCount);
        }

        public async Task<AdminPromotionDetailDto?> GetPromotionDetailAsync(Guid promotionId, CancellationToken cancellationToken)
        {
            return await _context.Promotions
                .AsNoTracking()
                .Where(p => p.Id == promotionId)
                .Select(p => new AdminPromotionDetailDto
                {
                    PromotionId = p.Id,
                    Name = p.Name,
                    DiscountType = p.DiscountType,
                    DiscountValue = p.DiscountValue,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    IsActive = p.IsActive,
                    Products = p.PromotionProducts.Select(pp => new PromotionProductItemDto
                    {
                        ProductId = pp.ProductId,
                        ProductName = pp.Product.Name
                    }).ToList(),
                    Banners = p.PromotionBanners.Select(pb => new PromotionBannerItemDto
                    {
                        BannerId = pb.Id,
                        ImageUrl = pb.ImageURL,
                        AltText = pb.AltText
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<(Promotion? Promotion, int ProductCount)> UpdatePromotionAsync(
            Guid promotionId,
            string name,
            DiscountType discountType,
            decimal discountValue,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken)
        {
            var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == promotionId, cancellationToken);

            if (promotion is null)
            {
                return (null, 0);
            }

            promotion.Name = name;
            promotion.DiscountType = discountType;
            promotion.DiscountValue = discountValue;
            promotion.StartDate = startDate;
            promotion.EndDate = endDate;

            await _context.SaveChangesAsync(cancellationToken);

            var productCount = await _context.PromotionProducts.CountAsync(pp => pp.PromotionId == promotionId, cancellationToken);

            return (promotion, productCount);
        }

        public async Task<Promotion?> UpdatePromotionStatusAsync(Guid promotionId, bool isActive, CancellationToken cancellationToken)
        {
            var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == promotionId, cancellationToken);

            if (promotion is null)
            {
                return null;
            }

            promotion.IsActive = isActive;
            await _context.SaveChangesAsync(cancellationToken);

            return promotion;
        }

        public async Task<bool> DeletePromotionAsync(Guid promotionId, CancellationToken cancellationToken)
        {
            var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == promotionId, cancellationToken);

            if (promotion is null)
            {
                return false;
            }

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<PromotionBanner?> AddPromotionBannerAsync(
            Guid promotionId,
            string imageUrl,
            string altText,
            CancellationToken cancellationToken)
        {
            var promotionExists = await _context.Promotions.AnyAsync(p => p.Id == promotionId, cancellationToken);

            if (!promotionExists)
            {
                return null;
            }

            var banner = new PromotionBanner
            {
                PromotionId = promotionId,
                ImageURL = imageUrl,
                AltText = altText
            };

            _context.PromotionBanners.Add(banner);
            await _context.SaveChangesAsync(cancellationToken);

            return banner;
        }

        public async Task<bool> RemovePromotionBannerAsync(Guid promotionId, Guid bannerId, CancellationToken cancellationToken)
        {
            var banner = await _context.PromotionBanners.FirstOrDefaultAsync(
                pb => pb.Id == bannerId && pb.PromotionId == promotionId, cancellationToken);

            if (banner is null)
            {
                return false;
            }

            _context.PromotionBanners.Remove(banner);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<(List<DiscountedProductDto> Items, int TotalCount)> GetDiscountedProductsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var activePromotionProductsQuery = _context.PromotionProducts
                .Where(pp => pp.Promotion.IsActive && pp.Promotion.StartDate <= now && pp.Promotion.EndDate >= now);

            var qualifyingProductIdsQuery = activePromotionProductsQuery.Select(pp => pp.ProductId).Distinct();

            var totalCount = await qualifyingProductIdsQuery.CountAsync(cancellationToken);

            var pageProductIds = await _context.Products
                .Where(p => qualifyingProductIdsQuery.Contains(p.Id))
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            var rawItems = await _context.Products
                .AsNoTracking()
                .Where(p => pageProductIds.Contains(p.Id))
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    Designs = p.Designs.Select(d => new
                    {
                        d.Id,
                        d.DesignName,
                        d.EstimatedPrice,
                        TotalQuantity = d.InventoryStocks.Sum(i => (int?)i.Quantity) ?? 0,
                        TotalSizes = d.Sizes.Count(),
                        Image = d.ProductImages
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
                    }).ToList(),
                    Candidates = p.PromotionProducts
                        .Where(pp => pp.Promotion.IsActive && pp.Promotion.StartDate <= now && pp.Promotion.EndDate >= now)
                        .Select(pp => new
                        {
                            pp.PromotionId,
                            pp.Promotion.DiscountType,
                            pp.Promotion.DiscountValue
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            var items = rawItems.Select(p =>
            {
                var priceBasis = p.Designs.Sum(d => d.EstimatedPrice);

                var best = p.Candidates
                    .OrderByDescending(c => c.DiscountType == DiscountType.Percentage
                        ? priceBasis * c.DiscountValue / 100m
                        : Math.Min(c.DiscountValue, priceBasis))
                    .First();

                decimal ApplyDiscount(decimal price) => best.DiscountType == DiscountType.Percentage
                    ? Math.Max(0, price - price * best.DiscountValue / 100m)
                    : Math.Max(0, price - best.DiscountValue);

                return new DiscountedProductDto
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Category = new CategoryDto { CategoryId = p.CategoryId, CategoryName = p.CategoryName },
                    Designs = p.Designs.Select(d => new DiscountedDesignDto
                    {
                        DesignId = d.Id,
                        DesignName = d.DesignName,
                        EstimatedPrice = d.EstimatedPrice,
                        DiscountedEstimatedPrice = ApplyDiscount(d.EstimatedPrice),
                        TotalQuantity = d.TotalQuantity,
                        TotalSizes = d.TotalSizes,
                        Image = d.Image
                    }).ToList(),
                    Promotion = new DiscountedProductPromotionDto
                    {
                        PromotionId = best.PromotionId,
                        DiscountType = best.DiscountType,
                        DiscountValue = best.DiscountValue,
                        DiscountTag = PromotionDisplayHelper.ComputeDiscountTag(best.DiscountType, best.DiscountValue)
                    }
                };
            }).ToList();

            return (items, totalCount);
        }

        public async Task<List<PublicPromotionBannerDto>> GetActivePromotionBannersAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            return await _context.PromotionBanners
                .AsNoTracking()
                .Where(pb => pb.Promotion.IsActive && pb.Promotion.StartDate <= now && pb.Promotion.EndDate >= now)
                .OrderBy(pb => pb.CreatedAt)
                .Select(pb => new PublicPromotionBannerDto
                {
                    BannerId = pb.Id,
                    ImageUrl = pb.ImageURL,
                    AltText = pb.AltText
                })
                .ToListAsync(cancellationToken);
        }
    }
}
