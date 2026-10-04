using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public interface IPromotionRepository
    {
        Task<(Promotion Promotion, int ProductCount)> CreatePromotionAsync(
            Promotion promotion,
            List<Guid> productIds,
            CancellationToken cancellationToken);

        Task<(List<(Promotion Promotion, int ProductCount, int BannerCount)> Items, int TotalCount)> GetAllPromotionsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<AdminPromotionDetailDto?> GetPromotionDetailAsync(Guid promotionId, CancellationToken cancellationToken);

        Task<(Promotion? Promotion, int ProductCount)> UpdatePromotionAsync(
            Guid promotionId,
            string name,
            DiscountType discountType,
            decimal discountValue,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken);

        Task<Promotion?> UpdatePromotionStatusAsync(Guid promotionId, bool isActive, CancellationToken cancellationToken);

        Task<bool> DeletePromotionAsync(Guid promotionId, CancellationToken cancellationToken);

        Task<PromotionBanner?> AddPromotionBannerAsync(
            Guid promotionId,
            string imageUrl,
            string altText,
            CancellationToken cancellationToken);

        Task<bool> RemovePromotionBannerAsync(Guid promotionId, Guid bannerId, CancellationToken cancellationToken);

        Task<(List<DiscountedProductDto> Items, int TotalCount)> GetDiscountedProductsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<List<PublicPromotionBannerDto>> GetActivePromotionBannersAsync(CancellationToken cancellationToken);
    }
}
