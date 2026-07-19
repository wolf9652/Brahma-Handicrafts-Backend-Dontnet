namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class SignUpRequest
    {
        public string Name { get; set; } 
        public string Email { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public int Role { get; set; } // 1 = Admin, 2 = Customer
        public string Password { get; set; } 
    }
}
