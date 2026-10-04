using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record UpdateProductCommand(Guid ProductId, UpdateProductRequest Request)
        : IRequest<(UpdateProductResult Result, CreatedProductDto? Product, string? ErrorMessage)>;

    public class UpdateProductCommandHandler
        : IRequestHandler<UpdateProductCommand, (UpdateProductResult Result, CreatedProductDto? Product, string? ErrorMessage)>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFileUploadService _fileUploadService;

        public UpdateProductCommandHandler(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IFileUploadService fileUploadService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<(UpdateProductResult Result, CreatedProductDto? Product, string? ErrorMessage)> Handle(
            UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (request.NewDesigns.Any(d => d.Sizes.Count == 0))
            {
                return (UpdateProductResult.DesignWouldHaveNoSizes, null, "Every new design must have at least one size.");
            }

            var data = new ProductUpdateData
            {
                ProductName = request.ProductName,
                IsActive = request.IsActive,
                RemoveDesignIds = request.RemoveDesignIds,
                RemoveImageIds = request.RemoveImageIds,
                RemoveSizeIds = request.RemoveSizeIds
            };

            if (!string.IsNullOrWhiteSpace(request.CategoryName))
            {
                var (categoryId, categoryName) = await _categoryRepository.GetOrCreateCategoryIdAsync(request.CategoryName, cancellationToken);
                data.CategoryId = categoryId;
                data.CategoryName = categoryName;
            }

            data.UpdateDesigns = request.UpdateDesigns.Select(d => new DesignUpdateData
            {
                DesignId = d.DesignId,
                DesignName = d.DesignName,
                DesignDescription = d.DesignDescription,
                EstimatedPrice = d.EstimatedPrice
            }).ToList();

            foreach (var imageEntry in request.NewImages)
            {
                var url = await _fileUploadService.SaveProductImageAsync(imageEntry.File, cancellationToken);
                data.NewImages.Add(new ImageAdditionData
                {
                    DesignId = imageEntry.DesignId,
                    Url = url,
                    AltText = imageEntry.AltText,
                    IsPrimary = imageEntry.IsPrimary
                });
            }

            foreach (var imageEntry in request.UpdateImages)
            {
                string? newUrl = null;
                if (imageEntry.File is not null)
                {
                    newUrl = await _fileUploadService.SaveProductImageAsync(imageEntry.File, cancellationToken);
                }

                data.UpdateImages.Add(new ImageUpdateData
                {
                    ImageId = imageEntry.ImageId,
                    NewUrl = newUrl,
                    AltText = imageEntry.AltText,
                    IsPrimary = imageEntry.IsPrimary
                });
            }

            data.NewSizes = request.NewSizes.Select(s => new SizeAdditionData
            {
                DesignId = s.DesignId,
                SizeName = s.SizeName,
                Length = s.Length,
                Breadth = s.Breadth,
                Height = s.Height,
                Weight = s.Weight,
                BasePrice = s.BasePrice,
                MRP = s.MRP,
                Quantity = s.Quantity
            }).ToList();

            data.UpdateSizes = request.UpdateSizes.Select(s => new SizeUpdateData
            {
                SizeId = s.SizeId,
                SizeName = s.SizeName,
                Length = s.Length,
                Breadth = s.Breadth,
                Height = s.Height,
                Weight = s.Weight,
                BasePrice = s.BasePrice,
                MRP = s.MRP,
                Quantity = s.Quantity
            }).ToList();

            foreach (var designRequest in request.NewDesigns)
            {
                var images = new List<ImageCreationData>();

                foreach (var imageRequest in designRequest.Images)
                {
                    var url = await _fileUploadService.SaveProductImageAsync(imageRequest.File, cancellationToken);
                    images.Add(new ImageCreationData
                    {
                        Url = url,
                        AltText = imageRequest.AltText,
                        IsPrimary = imageRequest.IsPrimary
                    });
                }

                data.NewDesigns.Add(new DesignCreationData
                {
                    DesignName = designRequest.DesignName,
                    DesignDescription = designRequest.DesignDescription,
                    EstimatedPrice = designRequest.EstimatedPrice,
                    Images = images,
                    Sizes = designRequest.Sizes.Select(s => new SizeCreationData
                    {
                        SizeName = s.SizeName,
                        Length = s.Length,
                        Breadth = s.Breadth,
                        Height = s.Height,
                        Weight = s.Weight,
                        BasePrice = s.BasePrice,
                        MRP = s.MRP,
                        Quantity = s.Quantity
                    }).ToList()
                });
            }

            return await _productRepository.UpdateProductAsync(command.ProductId, data, cancellationToken);
        }
    }
}
