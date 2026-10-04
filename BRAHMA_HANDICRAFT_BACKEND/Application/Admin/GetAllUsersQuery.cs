using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record GetAllUsersQuery(int PageNumber, int PageSize)
        : IRequest<PaginatedResponse<AdminUserDto>>;

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedResponse<AdminUserDto>>
    {
        private const int MaxPageSize = 50;

        private readonly IUserRepository _userRepository;

        public GetAllUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PaginatedResponse<AdminUserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? MaxPageSize : Math.Min(request.PageSize, MaxPageSize);

            var (items, totalCount) = await _userRepository.GetAllUsersAsync(pageNumber, pageSize, cancellationToken);

            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedResponse<AdminUserDto>
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
