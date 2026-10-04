using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class PromotionProductItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }

    public class PromotionBannerItemDto
    {
        public Guid BannerId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
    }

    public class AdminPromotionDetailDto
    {
        public Guid PromotionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountTag { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public List<PromotionProductItemDto> Products { get; set; } = new();
        public List<PromotionBannerItemDto> Banners { get; set; } = new();
    }
}
