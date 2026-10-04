namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class PublicPromotionBannerDto
    {
        public Guid BannerId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
    }
}
