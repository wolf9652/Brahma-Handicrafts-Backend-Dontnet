namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class AddToWishlistResponseDto
    {
        public Guid WishlistItemId { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid DesignId { get; set; }
    }
}
