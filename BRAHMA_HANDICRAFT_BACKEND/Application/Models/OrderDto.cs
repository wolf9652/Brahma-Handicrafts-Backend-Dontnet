using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class AddressSnapshotDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class OrderItemDto
    {
        public Guid OrderItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid DesignId { get; set; }
        public string DesignName { get; set; } = string.Empty;
        public Guid SizeId { get; set; }
        public string SizeName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public ProductImageDto? Image { get; set; }
    }

    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string? GSTNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public OrderStatus OrderStatus { get; set; }
        public decimal ItemsSubTotal { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal TotalTaxCharges { get; set; }
        public decimal GrandTotal { get; set; }
        public Guid ShippingAddressId { get; set; }
        public AddressSnapshotDto ShippingAddress { get; set; } = null!;
        public Guid BillingAddressId { get; set; }
        public AddressSnapshotDto BillingAddress { get; set; } = null!;
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
