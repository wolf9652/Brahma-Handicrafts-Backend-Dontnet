using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record GetAdminProductsQuery(bool IncludeInactive, int PageNumber, int PageSize)
        : IRequest<PaginatedResponse<AdminProductDto>>;

    public class GetAdminProductsQueryHandler : IRequestHandler<GetAdminProductsQuery, PaginatedResponse<AdminProductDto>>
    {
        private const int MaxPageSize = 50;

        private readonly IProductRepository _productRepository;

        public GetAdminProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PaginatedResponse<AdminProductDto>> Handle(GetAdminProductsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? MaxPageSize : Math.Min(request.PageSize, MaxPageSize);

            var (items, totalCount) = await _productRepository.GetAdminProductsAsync(
                request.IncludeInactive, pageNumber, pageSize, cancellationToken);

            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedResponse<AdminProductDto>
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
