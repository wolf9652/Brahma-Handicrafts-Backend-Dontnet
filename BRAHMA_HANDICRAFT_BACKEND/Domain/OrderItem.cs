namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? DesignId { get; set; }
        public Guid? SizeId { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }

        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public Design? Design { get; set; }
        public Size? Size { get; set; }
    }
}
