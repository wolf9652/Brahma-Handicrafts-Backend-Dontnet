using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record GetAllInventoryQuery(bool IncludeInactive, int PageNumber, int PageSize)
        : IRequest<PaginatedResponse<InventoryItemDto>>;

    public class GetAllInventoryQueryHandler : IRequestHandler<GetAllInventoryQuery, PaginatedResponse<InventoryItemDto>>
    {
        private const int MaxPageSize = 50;

        private readonly IInventoryRepository _inventoryRepository;

        public GetAllInventoryQueryHandler(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<PaginatedResponse<InventoryItemDto>> Handle(GetAllInventoryQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? MaxPageSize : Math.Min(request.PageSize, MaxPageSize);

            var (items, totalCount) = await _inventoryRepository.GetAllInventoryAsync(
                request.IncludeInactive, pageNumber, pageSize, cancellationToken);

            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedResponse<InventoryItemDto>
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
