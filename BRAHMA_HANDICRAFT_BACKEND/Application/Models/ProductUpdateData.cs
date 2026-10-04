namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class ProductUpdateData
    {
        public string? ProductName { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool? IsActive { get; set; }

        public List<DesignCreationData> NewDesigns { get; set; } = new();
        public List<DesignUpdateData> UpdateDesigns { get; set; } = new();
        public List<Guid> RemoveDesignIds { get; set; } = new();

        public List<ImageAdditionData> NewImages { get; set; } = new();
        public List<ImageUpdateData> UpdateImages { get; set; } = new();
        public List<Guid> RemoveImageIds { get; set; } = new();

        public List<SizeAdditionData> NewSizes { get; set; } = new();
        public List<SizeUpdateData> UpdateSizes { get; set; } = new();
        public List<Guid> RemoveSizeIds { get; set; } = new();
    }

    public class DesignUpdateData
    {
        public Guid DesignId { get; set; }
        public string DesignName { get; set; } = string.Empty;
        public string DesignDescription { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; }
    }

    public class ImageAdditionData
    {
        public Guid DesignId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    public class ImageUpdateData
    {
        public Guid ImageId { get; set; }
        public string? NewUrl { get; set; }
        public string AltText { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    public class SizeAdditionData
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

    public class SizeUpdateData
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
