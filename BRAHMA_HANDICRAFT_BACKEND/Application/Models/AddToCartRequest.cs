namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class AddToCartRequest
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid DesignId { get; set; }
        public Guid SizeId { get; set; }
        public int Quantity { get; set; }
    }
}
