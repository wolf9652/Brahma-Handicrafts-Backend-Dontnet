using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Cart
{
    public record GetCartItemsByUserIdQuery(Guid UserId)
        : IRequest<List<CartItemDto>>;

    public class GetCartItemsByUserIdQueryHandler : IRequestHandler<GetCartItemsByUserIdQuery, List<CartItemDto>>
    {
        private readonly ICartRepository _cartRepository;

        public GetCartItemsByUserIdQueryHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public Task<List<CartItemDto>> Handle(GetCartItemsByUserIdQuery request, CancellationToken cancellationToken)
        {
            return _cartRepository.GetCartItemsByUserIdAsync(request.UserId, cancellationToken);
        }
    }
}
