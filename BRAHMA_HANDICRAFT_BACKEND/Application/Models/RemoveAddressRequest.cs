namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class RemoveAddressRequest
    {
        public Guid UserId { get; set; }
        public Guid AddressId { get; set; }
    }
}
