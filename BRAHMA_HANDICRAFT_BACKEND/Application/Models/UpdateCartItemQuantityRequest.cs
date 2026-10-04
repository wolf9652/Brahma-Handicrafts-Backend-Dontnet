namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class UpdateCartItemQuantityRequest
    {
        public Guid UserId { get; set; }
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
