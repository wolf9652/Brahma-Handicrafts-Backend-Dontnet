using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Products
{
    public record GetProductsByCategoryIdQuery(Guid CategoryId)
        : IRequest<List<ProductByCategoryDto>>;

    public class GetProductsByCategoryIdQueryHandler : IRequestHandler<GetProductsByCategoryIdQuery, List<ProductByCategoryDto>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsByCategoryIdQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<List<ProductByCategoryDto>> Handle(GetProductsByCategoryIdQuery request, CancellationToken cancellationToken)
        {
            return _productRepository.GetProductsByCategoryIdAsync(request.CategoryId, cancellationToken);
        }
    }
}
