namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class AdminOrderDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public OrderDto Order { get; set; } = null!;
    }
}
