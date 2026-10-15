using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Products
{
    public record GetNewestProductDesignsQuery : IRequest<List<NewestProductDesignDto>>;

    public class GetNewestProductDesignsQueryHandler : IRequestHandler<GetNewestProductDesignsQuery, List<NewestProductDesignDto>>
    {
        private const int Count = 5;

        private readonly IProductRepository _productRepository;

        public GetNewestProductDesignsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<List<NewestProductDesignDto>> Handle(GetNewestProductDesignsQuery request, CancellationToken cancellationToken)
        {
            return _productRepository.GetNewestProductDesignsAsync(Count, cancellationToken);
        }
    }
}
