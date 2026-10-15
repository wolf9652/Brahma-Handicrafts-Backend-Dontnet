namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class CreatedProductDesignDto
    {
        public Guid DesignId { get; set; }
        public string DesignName { get; set; } = string.Empty;
        public string DesignDescription { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
        public List<SizeWithInventoryDto> Sizes { get; set; } = new();
    }

    public class CreatedProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public CategoryDto Category { get; set; } = null!;
        public List<CreatedProductDesignDto> Designs { get; set; } = new();
    }
}
