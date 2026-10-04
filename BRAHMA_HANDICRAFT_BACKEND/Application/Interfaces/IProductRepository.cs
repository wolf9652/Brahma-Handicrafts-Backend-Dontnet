using BRAHMA_HANDICRAFT_BACKEND.Application.Models;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public enum UpdateProductResult
    {
        Success,
        ProductNotFound,
        InvalidReference,
        WouldHaveNoDesigns,
        DesignWouldHaveNoSizes,
        ReferencedElsewhere
    }

    public interface IProductRepository
    {
        Task<List<ProductByCategoryDto>> GetProductsByCategoryIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken);

        Task<(List<ProductSummaryDto> Items, int TotalCount)> GetProductSummariesAsync(
            bool includeInactive,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<(List<AdminProductDto> Items, int TotalCount)> GetAdminProductsAsync(
            bool includeInactive,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<CreatedProductDto> CreateProductAsync(
            Guid categoryId,
            string categoryName,
            string productName,
            List<DesignCreationData> designs,
            CancellationToken cancellationToken);

        Task<CreatedProductDto?> GetProductDetailsAsync(Guid productId, CancellationToken cancellationToken);

        Task<List<NewestProductDesignDto>> GetNewestProductDesignsAsync(int count, CancellationToken cancellationToken);

        Task<(UpdateProductResult Result, CreatedProductDto? Product, string? ErrorMessage)> UpdateProductAsync(
            Guid productId,
            ProductUpdateData data,
            CancellationToken cancellationToken);
    }
}
