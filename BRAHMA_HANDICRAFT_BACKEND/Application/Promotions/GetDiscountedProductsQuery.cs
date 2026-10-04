using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Promotions
{
    public record GetDiscountedProductsQuery(int PageNumber, int PageSize)
        : IRequest<PaginatedResponse<DiscountedProductDto>>;

    public class GetDiscountedProductsQueryHandler : IRequestHandler<GetDiscountedProductsQuery, PaginatedResponse<DiscountedProductDto>>
    {
        private const int MaxPageSize = 50;

        private readonly IPromotionRepository _promotionRepository;

        public GetDiscountedProductsQueryHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<PaginatedResponse<DiscountedProductDto>> Handle(GetDiscountedProductsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? MaxPageSize : Math.Min(request.PageSize, MaxPageSize);

            var (items, totalCount) = await _promotionRepository.GetDiscountedProductsAsync(pageNumber, pageSize, cancellationToken);

            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedResponse<DiscountedProductDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }
    }
}
