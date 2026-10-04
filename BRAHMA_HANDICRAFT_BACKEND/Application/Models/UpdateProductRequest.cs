using Microsoft.AspNetCore.Http;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class UpdateProductRequest
    {
        public string? ProductName { get; set; }
        public string? CategoryName { get; set; }
        public bool? IsActive { get; set; }

        public List<CreateProductDesignRequest> NewDesigns { get; set; } = new();
        public List<UpdateProductDesignEntry> UpdateDesigns { get; set; } = new();
        public List<Guid> RemoveDesignIds { get; set; } = new();

        public List<NewProductImageEntry> NewImages { get; set; } = new();
        public List<UpdateProductImageEntry> UpdateImages { get; set; } = new();
        public List<Guid> RemoveImageIds { get; set; } = new();

        public List<NewProductSizeEntry> NewSizes { get; set; } = new();
        public List<UpdateProductSizeEntry> UpdateSizes { get; set; } = new();
        public List<Guid> RemoveSizeIds { get; set; } = new();
    }

    public class UpdateProductDesignEntry
    {
        public Guid DesignId { get; set; }
        public string DesignName { get; set; } = string.Empty;
        public string DesignDescription { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; }
    }

    public class NewProductImageEntry
    {
        public Guid DesignId { get; set; }
        public IFormFile File { get; set; } = null!;
        public string AltText { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    public class UpdateProductImageEntry
    {
        public Guid ImageId { get; set; }
        public IFormFile? File { get; set; }
        public string AltText { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    public class NewProductSizeEntry
    {
        public Guid DesignId { get; set; }
        public string SizeName { get; set; } = string.Empty;
        public decimal Length { get; set; }
        public decimal Breadth { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal BasePrice { get; set; }
        public decimal MRP { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateProductSizeEntry
    {
        public Guid SizeId { get; set; }
        public string SizeName { get; set; } = string.Empty;
        public decimal Length { get; set; }
        public decimal Breadth { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal BasePrice { get; set; }
        public decimal MRP { get; set; }
        public int Quantity { get; set; }
    }
}
