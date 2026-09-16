namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? GSTNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public decimal ItemsSubTotal { get; set; }
        public decimal ShippingCharges { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal TotalTaxCharges { get; set; }
        public decimal GrandTotal { get; set; }
        public Guid ShippingAddressId { get; set; }
        public string ShippingAddressJson { get; set; } = string.Empty;
        public Guid BillingAddressId { get; set; }
        public string BillingAddressJson { get; set; } = string.Empty;

        public Users User { get; set; } = null!;
        public Address ShippingAddress { get; set; } = null!;
        public Address BillingAddress { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
