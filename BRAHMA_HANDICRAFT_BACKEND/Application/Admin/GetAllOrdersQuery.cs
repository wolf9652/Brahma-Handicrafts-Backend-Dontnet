using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record GetAllOrdersQuery(int PageNumber, int PageSize) : IRequest<PaginatedResponse<AdminOrderDto>>;

    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, PaginatedResponse<AdminOrderDto>>
    {
        private const int MaxPageSize = 50;

        private readonly IOrderRepository _orderRepository;

        public GetAllOrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<PaginatedResponse<AdminOrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? MaxPageSize : Math.Min(request.PageSize, MaxPageSize);

            var (items, totalCount) = await _orderRepository.GetAllOrdersAsync(pageNumber, pageSize, cancellationToken);

            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedResponse<AdminOrderDto>
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
