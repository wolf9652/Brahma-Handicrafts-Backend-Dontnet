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
        private readonly IFileUploadService _fileUploadService;

        public PromotionRepository(AppDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<(CreatePromotionResult Result, PromotionDto? Promotion, List<Guid>? InvalidProductIds)> CreatePromotionAsync(
            string name,
            DiscountType discountType,
            decimal discountValue,
            DateTime startDate,
            DateTime endDate,
            bool isActive,
            List<Guid> productIds,
            List<PromotionBannerCreationData> banners,
            CancellationToken cancellationToken)
        {
            if (productIds.Count > 0)
            {
                var existingProductIds = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync(cancellationToken);

                var invalidProductIds = productIds.Except(existingProductIds).ToList();

                if (invalidProductIds.Count > 0)
                {
                    return (CreatePromotionResult.InvalidProductIds, null, invalidProductIds);
                }
            }

            var promotion = new Promotion
            {
                Id = Guid.NewGuid(),
                Name = name,
                DiscountType = discountType,
                DiscountValue = discountValue,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = isActive
            };
            _context.Promotions.Add(promotion);

            foreach (var productId in productIds)
            {
                _context.PromotionProducts.Add(new PromotionProduct
                {
                    Id = Guid.NewGuid(),
                    PromotionId = promotion.Id,
                    ProductId = productId
                });
            }

            var createdBanners = new List<PromotionBanner>();
            foreach (var bannerData in banners)
            {
                var banner = new PromotionBanner
                {
                    Id = Guid.NewGuid(),
                    PromotionId = promotion.Id,
                    ImageURL = bannerData.Url,
                    AltText = bannerData.AltText
                };
                _context.PromotionBanners.Add(banner);
                createdBanners.Add(banner);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var promotionDto = new PromotionDto
            {
                PromotionId = promotion.Id,
                Name = promotion.Name,
                DiscountType = promotion.DiscountType,
                DiscountValue = promotion.DiscountValue,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsActive = promotion.IsActive,
                ProductIds = productIds,
                Banners = createdBanners.Select(b => new PromotionBannerDto
                {
                    BannerId = b.Id,
                    Url = b.ImageURL,
                    AltText = b.AltText
                }).ToList()
            };

            return (CreatePromotionResult.Success, promotionDto, null);
        }

        public async Task<(List<AdminPromotionListItemDto> Items, int TotalCount)> GetAllPromotionsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Promotions.AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            var rawPromotions = await query
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.DiscountType,
                    p.DiscountValue,
                    p.StartDate,
                    p.EndDate,
                    p.IsActive,
                    ProductIds = p.PromotionProducts.Select(pp => pp.ProductId).ToList(),
                    Banners = p.PromotionBanners.Select(pb => new PromotionBannerDto
                    {
                        BannerId = pb.Id,
                        Url = pb.ImageURL,
                        AltText = pb.AltText
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;

            var items = rawPromotions.Select(p => new AdminPromotionListItemDto
            {
                PromotionId = p.Id,
                Name = p.Name,
                DiscountType = p.DiscountType,
                DiscountValue = p.DiscountValue,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                IsActive = p.IsActive,
                Status = ComputeStatus(p.IsActive, p.StartDate, p.EndDate, now),
                ProductCount = p.ProductIds.Count,
                ProductIds = p.ProductIds,
                Banners = p.Banners
            }).ToList();

            return (items, totalCount);
        }

        private static string ComputeStatus(bool isActive, DateTime startDate, DateTime endDate, DateTime now)
        {
            if (!isActive)
            {
                return "Inactive";
            }

            if (now < startDate)
            {
                return "Scheduled";
            }

            if (now > endDate)
            {
                return "Expired";
            }

            return "Active";
        }

        public async Task<(UpdatePromotionResult Result, PromotionDto? Promotion, List<Guid>? InvalidProductIds)> UpdatePromotionAsync(
            Guid promotionId,
            string? name,
            DiscountType? discountType,
            decimal? discountValue,
            DateTime? startDate,
            DateTime? endDate,
            bool? isActive,
            List<Guid>? productIds,
            List<PromotionBannerCreationData>? banners,
            CancellationToken cancellationToken)
        {
            var promotion = await _context.Promotions
                .Include(p => p.PromotionProducts)
                .Include(p => p.PromotionBanners)
                .FirstOrDefaultAsync(p => p.Id == promotionId, cancellationToken);

            if (promotion is null)
            {
                return (UpdatePromotionResult.PromotionNotFound, null, null);
            }

            if (productIds is not null && productIds.Count > 0)
            {
                var existingProductIds = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync(cancellationToken);

                var invalidProductIds = productIds.Except(existingProductIds).ToList();

                if (invalidProductIds.Count > 0)
                {
                    return (UpdatePromotionResult.InvalidProductIds, null, invalidProductIds);
                }
            }

            if (name is not null)
            {
                promotion.Name = name;
            }

            if (discountType.HasValue)
            {
                promotion.DiscountType = discountType.Value;
            }

            if (discountValue.HasValue)
            {
                promotion.DiscountValue = discountValue.Value;
            }

            if (startDate.HasValue)
            {
                promotion.StartDate = startDate.Value;
            }

            if (endDate.HasValue)
            {
                promotion.EndDate = endDate.Value;
            }

            if (isActive.HasValue)
            {
                promotion.IsActive = isActive.Value;
            }

            if (productIds is not null)
            {
                _context.PromotionProducts.RemoveRange(promotion.PromotionProducts);
                promotion.PromotionProducts.Clear();

                foreach (var productId in productIds)
                {
                    var promotionProduct = new PromotionProduct
                    {
                        Id = Guid.NewGuid(),
                        PromotionId = promotion.Id,
                        ProductId = productId
                    };
                    promotion.PromotionProducts.Add(promotionProduct);
                    _context.PromotionProducts.Add(promotionProduct);
                }
            }

            var filesToDelete = new List<string>();

            if (banners is not null)
            {
                filesToDelete.AddRange(promotion.PromotionBanners.Select(b => b.ImageURL));

                _context.PromotionBanners.RemoveRange(promotion.PromotionBanners);
                promotion.PromotionBanners.Clear();

                foreach (var bannerData in banners)
                {
                    var banner = new PromotionBanner
                    {
                        Id = Guid.NewGuid(),
                        PromotionId = promotion.Id,
                        ImageURL = bannerData.Url,
                        AltText = bannerData.AltText
                    };
                    promotion.PromotionBanners.Add(banner);
                    _context.PromotionBanners.Add(banner);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            foreach (var url in filesToDelete)
            {
                _fileUploadService.DeleteProductImage(url);
            }

            var promotionDto = new PromotionDto
            {
                PromotionId = promotion.Id,
                Name = promotion.Name,
                DiscountType = promotion.DiscountType,
                DiscountValue = promotion.DiscountValue,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsActive = promotion.IsActive,
                ProductIds = promotion.PromotionProducts.Select(pp => pp.ProductId).ToList(),
                Banners = promotion.PromotionBanners.Select(b => new PromotionBannerDto
                {
                    BannerId = b.Id,
                    Url = b.ImageURL,
                    AltText = b.AltText
                }).ToList()
            };

            return (UpdatePromotionResult.Success, promotionDto, null);
        }

        public async Task<(TogglePromotionActiveResult Result, bool IsActive)> TogglePromotionActiveAsync(
            Guid promotionId,
            CancellationToken cancellationToken)
        {
            var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == promotionId, cancellationToken);

            if (promotion is null)
            {
                return (TogglePromotionActiveResult.PromotionNotFound, false);
            }

            promotion.IsActive = !promotion.IsActive;
            await _context.SaveChangesAsync(cancellationToken);

            return (TogglePromotionActiveResult.Success, promotion.IsActive);
        }

        public async Task<bool> DeletePromotionAsync(Guid promotionId, CancellationToken cancellationToken)
        {
            var promotion = await _context.Promotions
                .Include(p => p.PromotionBanners)
                .FirstOrDefaultAsync(p => p.Id == promotionId, cancellationToken);

            if (promotion is null)
            {
                return false;
            }

            var bannerUrls = promotion.PromotionBanners.Select(b => b.ImageURL).ToList();

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync(cancellationToken);

            foreach (var url in bannerUrls)
            {
                _fileUploadService.DeleteProductImage(url);
            }

            return true;
        }

        public async Task<List<CustomerPromotionDto>> GetActivePromotionsAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            return await _context.Promotions
                .AsNoTracking()
                .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new CustomerPromotionDto
                {
                    PromotionId = p.Id,
                    DiscountType = p.DiscountType,
                    DiscountValue = p.DiscountValue,
                    ProductIds = p.PromotionProducts.Select(pp => pp.ProductId).ToList(),
                    Banners = p.PromotionBanners.Select(pb => new PromotionBannerDto
                    {
                        BannerId = pb.Id,
                        Url = pb.ImageURL,
                        AltText = pb.AltText
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
