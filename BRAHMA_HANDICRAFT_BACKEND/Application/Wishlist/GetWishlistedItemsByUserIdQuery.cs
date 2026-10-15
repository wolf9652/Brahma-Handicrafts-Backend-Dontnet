using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Wishlist
{
    public record GetWishlistedItemsByUserIdQuery(Guid UserId)
        : IRequest<List<WishlistItemDto>>;

    public class GetWishlistedItemsByUserIdQueryHandler : IRequestHandler<GetWishlistedItemsByUserIdQuery, List<WishlistItemDto>>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public GetWishlistedItemsByUserIdQueryHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public Task<List<WishlistItemDto>> Handle(GetWishlistedItemsByUserIdQuery request, CancellationToken cancellationToken)
        {
            return _wishlistRepository.GetWishlistedItemsByUserIdAsync(request.UserId, cancellationToken);
        }
    }
}
