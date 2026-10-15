using Microsoft.AspNetCore.Http;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class CreateProductRequest
    {
        public string CategoryName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public List<CreateProductDesignRequest> Designs { get; set; } = new();
    }

    public class CreateProductDesignRequest
    {
        public string DesignName { get; set; } = string.Empty;
        public string DesignDescription { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; }
        public List<CreateProductImageRequest> Images { get; set; } = new();
        public List<CreateProductSizeRequest> Sizes { get; set; } = new();
    }

    public class CreateProductImageRequest
    {
        public IFormFile File { get; set; } = null!;
        public string AltText { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    public class CreateProductSizeRequest
    {
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
