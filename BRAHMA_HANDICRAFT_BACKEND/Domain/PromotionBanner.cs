namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class PromotionBanner : BaseEntity
    {
        public Guid PromotionId { get; set; }
        public string ImageURL { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;

        public Promotion Promotion { get; set; } = null!;
    }
}
