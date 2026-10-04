using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public static class PromotionDisplayHelper
    {
        public static string ComputeDiscountTag(DiscountType discountType, decimal discountValue)
        {
            var value = discountValue.ToString("0.##");
            return discountType == DiscountType.Percentage ? $"{value}% OFF" : $"₹{value} OFF";
        }

        public static string ComputeStatus(bool isActive, DateTime startDate, DateTime endDate)
        {
            if (!isActive)
            {
                return "Inactive";
            }

            var now = DateTime.UtcNow;

            if (now < startDate)
            {
                return "Scheduled";
            }

            if (now > endDate)
            {
                return "Expired";
            }

            return "Active";
        }
    }
}
