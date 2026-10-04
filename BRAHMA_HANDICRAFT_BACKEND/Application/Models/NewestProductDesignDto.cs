namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class NewestProductDesignDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid DesignId { get; set; }
        public string DesignName { get; set; } = string.Empty;
        public string DesignDescription { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; }
        public ProductImageDto? Image { get; set; }
    }
}
