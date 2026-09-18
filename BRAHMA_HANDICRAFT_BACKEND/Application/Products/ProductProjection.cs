namespace BRAHMA_HANDICRAFT_BACKEND.Application.Products
{
    // Raw, EF-projected shape returned by IProductRepository. Always carries every branch
    // (Designs, product-level Sizes, product-level Images, product-level Inventory) regardless
    // of which of the 3 product scenarios applies; GetAllProductsQueryHandler decides which
    // branch to surface once the page is materialized.
    public class ProductProjection
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal DefaultPrice { get; set; }
        public decimal DefaultMRP { get; set; }
        public bool IsActive { get; set; }

        public List<DesignProjection> Designs { get; set; } = new();
        public List<SizeProjection> ProductSizes { get; set; } = new();
        public List<ImageProjection> ProductImages { get; set; } = new();
        public InventoryProjection? ProductInventory { get; set; }
    }

    public class DesignProjection
    {
        public Guid Id { get; set; }
        public string DesignName { get; set; } = string.Empty;
        public string DesignDescription { get; set; } = string.Empty;
        public List<SizeProjection> Sizes { get; set; } = new();
        public List<ImageProjection> Images { get; set; } = new();
    }

    public class SizeProjection
    {
        public Guid Id { get; set; }
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

    public class ImageProjection
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    public class InventoryProjection
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
    }
}
