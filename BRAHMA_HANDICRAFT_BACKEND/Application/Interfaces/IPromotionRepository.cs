using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public enum CreatePromotionResult
    {
        Success,
        InvalidProductIds
    }

    public enum UpdatePromotionResult
    {
        Success,
        PromotionNotFound,
        InvalidProductIds
    }

    public enum TogglePromotionActiveResult
    {
        Success,
        PromotionNotFound
    }

    public interface IPromotionRepository
    {
        Task<(CreatePromotionResult Result, PromotionDto? Promotion, List<Guid>? InvalidProductIds)> CreatePromotionAsync(
            string name,
            DiscountType discountType,
            decimal discountValue,
            DateTime startDate,
            DateTime endDate,
            bool isActive,
            List<Guid> productIds,
            List<PromotionBannerCreationData> banners,
            CancellationToken cancellationToken);

        Task<(List<AdminPromotionListItemDto> Items, int TotalCount)> GetAllPromotionsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<(UpdatePromotionResult Result, PromotionDto? Promotion, List<Guid>? InvalidProductIds)> UpdatePromotionAsync(
            Guid promotionId,
            string? name,
            DiscountType? discountType,
            decimal? discountValue,
            DateTime? startDate,
            DateTime? endDate,
            bool? isActive,
            List<Guid>? productIds,
            List<PromotionBannerCreationData>? banners,
            CancellationToken cancellationToken);

        Task<(TogglePromotionActiveResult Result, bool IsActive)> TogglePromotionActiveAsync(
            Guid promotionId,
            CancellationToken cancellationToken);

        Task<bool> DeletePromotionAsync(Guid promotionId, CancellationToken cancellationToken);

        Task<List<CustomerPromotionDto>> GetActivePromotionsAsync(CancellationToken cancellationToken);
    }
}
