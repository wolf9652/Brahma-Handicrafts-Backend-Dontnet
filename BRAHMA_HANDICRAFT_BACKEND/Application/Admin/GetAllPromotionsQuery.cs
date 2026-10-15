using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record GetAllPromotionsQuery(int PageNumber, int PageSize)
        : IRequest<PaginatedResponse<AdminPromotionListItemDto>>;

    public class GetAllPromotionsQueryHandler : IRequestHandler<GetAllPromotionsQuery, PaginatedResponse<AdminPromotionListItemDto>>
    {
        private const int MaxPageSize = 50;

        private readonly IPromotionRepository _promotionRepository;

        public GetAllPromotionsQueryHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<PaginatedResponse<AdminPromotionListItemDto>> Handle(GetAllPromotionsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? MaxPageSize : Math.Min(request.PageSize, MaxPageSize);

            var (items, totalCount) = await _promotionRepository.GetAllPromotionsAsync(pageNumber, pageSize, cancellationToken);

            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedResponse<AdminPromotionListItemDto>
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
