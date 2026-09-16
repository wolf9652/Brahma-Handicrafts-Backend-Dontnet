namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class ProductImage : BaseEntity
    {
        public string ImageURL { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public Guid? DesignId { get; set; }
        public string AltText { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }

        public Product Product { get; set; } = null!;
        public Design? Design { get; set; }
    }
}
