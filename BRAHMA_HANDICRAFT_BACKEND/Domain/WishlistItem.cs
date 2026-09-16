namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class WishlistItem : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? DesignId { get; set; }
        public Guid? SizeId { get; set; }

        public Users User { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public Design? Design { get; set; }
        public Size? Size { get; set; }
    }
}
