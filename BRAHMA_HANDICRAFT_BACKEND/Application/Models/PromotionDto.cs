using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class PromotionBannerDto
    {
        public Guid BannerId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
    }

    public class PromotionDto
    {
        public Guid PromotionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> ProductIds { get; set; } = new();
        public List<PromotionBannerDto> Banners { get; set; } = new();
    }
}
