using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Wishlist
{
    public record AddToWishlistCommand(Guid UserId, Guid ProductId, Guid DesignId)
        : IRequest<Guid>;

    public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand, Guid>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public AddToWishlistCommandHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public Task<Guid> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
        {
            var wishlistItem = new WishlistItem
            {
                UserId = request.UserId,
                ProductId = request.ProductId,
                DesignId = request.DesignId
            };

            return _wishlistRepository.AddToWishlistAsync(wishlistItem, cancellationToken);
        }
    }
}
