namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class PromotionProduct : BaseEntity
    {
        public Guid PromotionId { get; set; }
        public Guid ProductId { get; set; }

        public Promotion Promotion { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
