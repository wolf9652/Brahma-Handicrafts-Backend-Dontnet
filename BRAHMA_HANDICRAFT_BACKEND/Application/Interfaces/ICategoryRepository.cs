using BRAHMA_HANDICRAFT_BACKEND.Application.Models;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken);

        Task<(Guid CategoryId, string CategoryName)> GetOrCreateCategoryIdAsync(string categoryName, CancellationToken cancellationToken);
    }
}
