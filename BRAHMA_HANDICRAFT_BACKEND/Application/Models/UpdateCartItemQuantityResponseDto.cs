namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class UpdateCartItemQuantityResponseDto
    {
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
