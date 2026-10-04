namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class AdminProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public ProductImageDto? Image { get; set; }
        public int TotalQuantity { get; set; }
        public int DesignCount { get; set; }
    }
}
