namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class AddPromotionBannerRequest
    {
        public Guid PromotionId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
    }
}
