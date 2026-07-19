namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class UsersCredentials
    {
        public int CredentialId { get; set; }   // optional PK
        public int UserId { get; set; }         // FK → Users
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime? LastLoginDTM { get; set; }

        public Users User { get; set; }          // navigation property
    }
}
