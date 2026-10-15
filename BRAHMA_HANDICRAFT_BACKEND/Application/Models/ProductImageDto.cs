namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class ProductImageDto
    {
        public Guid ImageId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}
