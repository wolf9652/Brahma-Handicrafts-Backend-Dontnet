using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class DiscountedDesignDto
    {
        public Guid DesignId { get; set; }
        public string DesignName { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; }
        public decimal DiscountedEstimatedPrice { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalSizes { get; set; }
        public ProductImageDto? Image { get; set; }
    }

    public class DiscountedProductPromotionDto
    {
        public Guid PromotionId { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountTag { get; set; } = string.Empty;
    }

    public class DiscountedProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public CategoryDto Category { get; set; } = null!;
        public List<DiscountedDesignDto> Designs { get; set; } = new();
        public DiscountedProductPromotionDto Promotion { get; set; } = null!;
    }
}
