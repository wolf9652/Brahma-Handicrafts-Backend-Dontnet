using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.AspNetCore.Http;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Models
{
    public class CreatePromotionRequest
    {
        public string Name { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> ProductIds { get; set; } = new();
        public List<CreatePromotionBannerRequest> Banners { get; set; } = new();
    }

    public class CreatePromotionBannerRequest
    {
        public IFormFile File { get; set; } = null!;
        public string AltText { get; set; } = string.Empty;
    }
}
