using Microsoft.AspNetCore.Http;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> SaveProductImageAsync(IFormFile file, CancellationToken cancellationToken);

        void DeleteProductImage(string relativeUrl);
    }
}
