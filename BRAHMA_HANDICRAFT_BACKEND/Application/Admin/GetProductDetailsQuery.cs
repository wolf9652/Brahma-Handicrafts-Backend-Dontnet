using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record GetProductDetailsQuery(Guid ProductId) : IRequest<CreatedProductDto?>;

    public class GetProductDetailsQueryHandler : IRequestHandler<GetProductDetailsQuery, CreatedProductDto?>
    {
        private readonly IProductRepository _productRepository;

        public GetProductDetailsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<CreatedProductDto?> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
        {
            return _productRepository.GetProductDetailsAsync(request.ProductId, cancellationToken);
        }
    }
}
