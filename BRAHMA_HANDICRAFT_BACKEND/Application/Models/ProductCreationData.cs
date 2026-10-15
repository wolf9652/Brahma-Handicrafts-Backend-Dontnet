namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class DesignCreationData
    {
        public string DesignName { get; set; } = string.Empty;
        public string DesignDescription { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; }
        public List<ImageCreationData> Images { get; set; } = new();
        public List<SizeCreationData> Sizes { get; set; } = new();
    }

    public class ImageCreationData
    {
        public string Url { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    public class SizeCreationData
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
