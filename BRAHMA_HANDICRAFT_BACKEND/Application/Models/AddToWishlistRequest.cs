namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class AddToWishlistRequest
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid DesignId { get; set; }
    }
}
