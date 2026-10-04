namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class DesignSummaryDto
    {
        public Guid DesignId { get; set; }
        public string DesignName { get; set; } = string.Empty;
        public string DesignDescription { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalSizes { get; set; }
        public ProductImageDto? Image { get; set; }
    }

    public class ProductByCategoryDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<DesignSummaryDto> Designs { get; set; } = new();
    }
}
