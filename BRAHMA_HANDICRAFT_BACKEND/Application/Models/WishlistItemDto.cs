namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class WishlistItemDto
    {
        public Guid WishlistItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DesignSummaryDto Design { get; set; } = null!;
    }
}
