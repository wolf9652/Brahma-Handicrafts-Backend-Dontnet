namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class CreateOrderRequest
    {
        public Guid UserId { get; set; }
        public string? GSTNumber { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid BillingAddressId { get; set; }
        public decimal ItemsSubTotal { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal TotalTaxCharges { get; set; }
        public decimal GrandTotal { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }

    public class CreateOrderItemRequest
    {
        public Guid ProductId { get; set; }
        public Guid DesignId { get; set; }
        public Guid SizeId { get; set; }
        public decimal BasePrice { get; set; }
        public int Quantity { get; set; }
    }
}
