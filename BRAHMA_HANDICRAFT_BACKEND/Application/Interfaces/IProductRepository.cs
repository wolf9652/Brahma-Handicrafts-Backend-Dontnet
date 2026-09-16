using BRAHMA_HANDICRAFT_BACKEND.Application.Products;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<(List<ProductProjection> Items, int TotalCount)> GetProductsAsync(
            bool includeInactive,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
