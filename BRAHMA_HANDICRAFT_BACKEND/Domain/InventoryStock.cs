namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class InventoryStock : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid? SizeId { get; set; }
        public Guid? DesignId { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; }

        public Product Product { get; set; } = null!;
        public Size? Size { get; set; }
        public Design? Design { get; set; }
    }
}
