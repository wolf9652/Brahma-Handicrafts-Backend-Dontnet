namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class SizeWithInventoryDto
    {
        public Guid SizeId { get; set; }
        public string SizeName { get; set; } = string.Empty;
        public decimal Length { get; set; }
        public decimal Breadth { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal BasePrice { get; set; }
        public decimal MRP { get; set; }
        public Guid? InventoryStockId { get; set; }
        public int? Quantity { get; set; }
    }

    public class DesignDetailsDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid DesignId { get; set; }
        public string DesignName { get; set; } = string.Empty;
        public string DesignDescription { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
        public List<SizeWithInventoryDto> Sizes { get; set; } = new();
    }
}
