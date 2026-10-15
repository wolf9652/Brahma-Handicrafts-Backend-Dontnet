namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class Promotion : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();
        public ICollection<PromotionBanner> PromotionBanners { get; set; } = new List<PromotionBanner>();
    }
}
