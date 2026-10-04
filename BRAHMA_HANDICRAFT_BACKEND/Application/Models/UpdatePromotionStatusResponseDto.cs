namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class UpdatePromotionStatusResponseDto
    {
        public Guid PromotionId { get; set; }
        public bool IsActive { get; set; }
    }
}
