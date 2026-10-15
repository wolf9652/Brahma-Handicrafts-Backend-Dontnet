namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class SignUpRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool Role { get; set; } = true; // true = Customer, false = Admin
        public string Password { get; set; } = string.Empty;
    }
}
