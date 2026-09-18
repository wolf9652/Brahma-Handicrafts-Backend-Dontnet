namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class Design : BaseEntity
    {
        public string DesignName { get; set; } = string.Empty;
        public string DesignDescription { get; set; } = string.Empty;
        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;
        public ICollection<Size> Sizes { get; set; } = new List<Size>();
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
