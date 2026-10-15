namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class SignupResponseDto
    {
        public Guid UserId { get; set; }
        public string EmailId { get; set; } = string.Empty;
        public bool Role { get; set; }
    }
}
