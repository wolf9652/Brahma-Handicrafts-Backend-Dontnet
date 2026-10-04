using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Data;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.Id,
                    CategoryName = c.CategoryName
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<(Guid CategoryId, string CategoryName)> GetOrCreateCategoryIdAsync(string categoryName, CancellationToken cancellationToken)
        {
            var existing = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryName.ToLower(), cancellationToken);

            if (existing is not null)
            {
                return (existing.Id, existing.CategoryName);
            }

            var category = new Category { CategoryName = categoryName };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return (category.Id, category.CategoryName);
        }
    }
}
