namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class AdminUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public int OrdersCount { get; set; }
    }
}
