namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class Users : BaseEntity
    {
        public int UserId { get; set; }   // auto-increment by default
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
        public int Role { get; set; } = 2; // 1: Admin, 2: Customer
    }
}
