namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class AddToCartResponseDto
    {
        public Guid CartItemId { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid DesignId { get; set; }
        public Guid SizeId { get; set; }
    }
}
