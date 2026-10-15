using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Wishlist
{
    public record DeleteFromWishlistCommand(Guid UserId, Guid WishlistItemId)
        : IRequest<bool>;

    public class DeleteFromWishlistCommandHandler : IRequestHandler<DeleteFromWishlistCommand, bool>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public DeleteFromWishlistCommandHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public Task<bool> Handle(DeleteFromWishlistCommand request, CancellationToken cancellationToken)
        {
            return _wishlistRepository.DeleteFromWishlistAsync(request.UserId, request.WishlistItemId, cancellationToken);
        }
    }
}
