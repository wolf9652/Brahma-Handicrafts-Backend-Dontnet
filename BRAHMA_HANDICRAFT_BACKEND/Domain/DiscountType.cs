using System.Text.Json.Serialization;

namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }
}
