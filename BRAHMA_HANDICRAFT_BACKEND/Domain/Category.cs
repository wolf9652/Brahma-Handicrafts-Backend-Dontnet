namespace BRAHMA_HANDICRAFT_BACKEND.Domain
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
