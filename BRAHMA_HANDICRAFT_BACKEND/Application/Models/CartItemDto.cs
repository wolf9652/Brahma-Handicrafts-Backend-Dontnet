namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class CartItemDto
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid DesignId { get; set; }
        public Guid SizeId { get; set; }
        public string SizeName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int Quantity { get; set; }
        public DesignSummaryDto Design { get; set; } = null!;
    }
}
