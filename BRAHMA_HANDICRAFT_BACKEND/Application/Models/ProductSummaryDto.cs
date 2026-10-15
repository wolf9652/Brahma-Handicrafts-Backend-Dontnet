namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class ProductSummaryDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public CategoryDto Category { get; set; } = null!;
        public List<DesignSummaryDto> Designs { get; set; } = new();
    }
}
