using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public enum CreateProductResult
    {
        Success,
        NoDesigns,
        DesignWithNoSizes
    }

    public record CreateProductCommand(CreateProductRequest Request)
        : IRequest<(CreateProductResult Result, CreatedProductDto? Product)>;

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, (CreateProductResult Result, CreatedProductDto? Product)>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IFileUploadService _fileUploadService;

        public CreateProductCommandHandler(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IFileUploadService fileUploadService)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<(CreateProductResult Result, CreatedProductDto? Product)> Handle(
            CreateProductCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (request.Designs.Count == 0)
            {
                return (CreateProductResult.NoDesigns, null);
            }

            if (request.Designs.Any(d => d.Sizes.Count == 0))
            {
                return (CreateProductResult.DesignWithNoSizes, null);
            }

            var (categoryId, categoryName) = await _categoryRepository.GetOrCreateCategoryIdAsync(request.CategoryName, cancellationToken);

            var designs = new List<DesignCreationData>();

            foreach (var designRequest in request.Designs)
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

                designs.Add(new DesignCreationData
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

            var created = await _productRepository.CreateProductAsync(
                categoryId, categoryName, request.ProductName, designs, cancellationToken);

            return (CreateProductResult.Success, created);
        }
    }
}
