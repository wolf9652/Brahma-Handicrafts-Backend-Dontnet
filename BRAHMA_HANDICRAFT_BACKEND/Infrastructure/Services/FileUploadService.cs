using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Services
{
    public class FileUploadService : IFileUploadService
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };

        private const long MaxFileSizeBytes = 10 * 1024 * 1024;

        private readonly IWebHostEnvironment _environment;

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveProductImageAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if (file.Length == 0)
            {
                throw new ArgumentException("Uploaded file is empty.");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                throw new ArgumentException("Uploaded file exceeds the 10MB size limit.");
            }

            var extension = Path.GetExtension(file.FileName);

            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Unsupported image file type.");
            }

            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return $"uploads/{fileName}";
        }

        public void DeleteProductImage(string relativeUrl)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, relativeUrl.Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch
            {
                // Best-effort cleanup only; a failed physical delete should not fail the request.
            }
        }
    }
}
