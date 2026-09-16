namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string? ResponseCode { get; set; }
        public string? ErrorMessage { get; set; }

        public Order Order { get; set; } = null!;
    }
}
