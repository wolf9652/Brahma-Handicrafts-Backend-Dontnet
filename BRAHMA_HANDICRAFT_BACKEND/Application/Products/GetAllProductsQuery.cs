using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Products.Dtos;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Products
{
    public record GetAllProductsQuery(bool IncludeInactive, int PageNumber, int PageSize)
        : IRequest<PaginatedResponse<ProductDto>>;

    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginatedResponse<ProductDto>>
    {
        private const int MaxPageSize = 50;

        private readonly IProductRepository _productRepository;

        public GetAllProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PaginatedResponse<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? MaxPageSize : Math.Min(request.PageSize, MaxPageSize);

            var (items, totalCount) = await _productRepository.GetProductsAsync(
                request.IncludeInactive, pageNumber, pageSize, cancellationToken);

            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedResponse<ProductDto>
            {
                Items = items.Select(MapToDto).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }

        private static ProductDto MapToDto(ProductProjection p)
        {
            var dto = new ProductDto
            {
                ProductId = p.Id,
                Name = p.Name,
                Description = p.Description,
                Category = new CategoryDto { CategoryId = p.CategoryId, CategoryName = p.CategoryName },
                DefaultPrice = p.DefaultPrice,
                DefaultMRP = p.DefaultMRP,
                IsActive = p.IsActive
            };

            if (p.Designs.Count > 0)
            {
                dto.Designs = p.Designs.Select(d => new DesignDto
                {
                    DesignId = d.Id,
                    DesignName = d.DesignName,
                    DesignDescription = d.DesignDescription,
                    Sizes = d.Sizes.Select(MapSize).ToList(),
                    Images = d.Images.Select(MapImage).ToList()
                }).ToList();
            }
            else if (p.ProductSizes.Count > 0)
            {
                dto.Sizes = p.ProductSizes.Select(MapSize).ToList();
                dto.Images = p.ProductImages.Select(MapImage).ToList();
            }
            else
            {
                dto.InventoryStockId = p.ProductInventory?.Id;
                dto.Quantity = p.ProductInventory?.Quantity;
                dto.Images = p.ProductImages.Select(MapImage).ToList();
            }

            return dto;
        }

        private static SizeDto MapSize(SizeProjection s) => new()
        {
            SizeId = s.Id,
            SizeName = s.SizeName,
            Length = s.Length,
            Breadth = s.Breadth,
            Height = s.Height,
            Weight = s.Weight,
            BasePrice = s.BasePrice,
            MRP = s.MRP,
            InventoryStockId = s.InventoryStockId,
            Quantity = s.Quantity
        };

        private static ProductImageDto MapImage(ImageProjection i) => new()
        {
            ImageId = i.Id,
            Url = i.Url,
            AltText = i.AltText,
            IsPrimary = i.IsPrimary
        };
    }
}
