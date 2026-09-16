using System.Text.Json.Serialization;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Products.Dtos
{
    public class CategoryDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class ProductImageDto
    {
        public Guid ImageId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    public class SizeDto
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

    public class DesignDto
    {
        public Guid DesignId { get; set; }
        public string DesignName { get; set; } = string.Empty;
        public string DesignDescription { get; set; } = string.Empty;
        public List<SizeDto> Sizes { get; set; } = new();
        public List<ProductImageDto> Images { get; set; } = new();
    }

    // Designs, Sizes, Images, InventoryStockId and Quantity are mutually exclusive depending on
    // the product's scenario (Design->Size / Size-only / neither) and are omitted from the
    // serialized response when null, rather than emitted as empty arrays/nulls.
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CategoryDto Category { get; set; } = null!;
        public decimal DefaultPrice { get; set; }
        public decimal DefaultMRP { get; set; }
        public bool IsActive { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<DesignDto>? Designs { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<SizeDto>? Sizes { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ProductImageDto>? Images { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? InventoryStockId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Quantity { get; set; }
    }
}
