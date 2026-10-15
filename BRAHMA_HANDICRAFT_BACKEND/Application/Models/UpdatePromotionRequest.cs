using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class UpdatePromotionRequest
    {
        public string? Name { get; set; }
        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public List<Guid>? ProductIds { get; set; }
        public List<CreatePromotionBannerRequest>? Banners { get; set; }
    }
}
