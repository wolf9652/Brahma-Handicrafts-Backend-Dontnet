namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class InventoryItemDto
    {
        public Guid InventoryStockId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid DesignId { get; set; }
        public string DesignName { get; set; } = string.Empty;
        public Guid SizeId { get; set; }
        public string SizeName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public ProductImageDto? Image { get; set; }
    }
}
