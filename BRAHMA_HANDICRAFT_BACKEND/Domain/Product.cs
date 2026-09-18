namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public decimal DefaultPrice { get; set; }
        public decimal DefaultMRP { get; set; }
        public bool IsActive { get; set; } = true;

        public Category Category { get; set; } = null!;
        public ICollection<Design> Designs { get; set; } = new List<Design>();
        public ICollection<Size> Sizes { get; set; } = new List<Size>();
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
