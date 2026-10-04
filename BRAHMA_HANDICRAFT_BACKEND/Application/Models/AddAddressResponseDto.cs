namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class AddAddressResponseDto
    {
        public Guid AddressId { get; set; }
        public Guid UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; } = string.Empty;
    }
}
