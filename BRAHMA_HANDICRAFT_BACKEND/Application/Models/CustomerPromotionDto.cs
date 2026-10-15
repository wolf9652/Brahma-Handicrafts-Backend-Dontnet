using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class CustomerPromotionDto
    {
        public Guid PromotionId { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public List<Guid> ProductIds { get; set; } = new();
        public List<PromotionBannerDto> Banners { get; set; } = new();
    }
}
