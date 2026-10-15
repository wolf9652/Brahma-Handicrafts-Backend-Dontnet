using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Cart
{
    public record UpdateCartItemQuantityCommand(Guid UserId, Guid CartItemId, int Quantity)
        : IRequest<UpdateCartItemQuantityResult>;

    public class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand, UpdateCartItemQuantityResult>
    {
        private readonly ICartRepository _cartRepository;

        public UpdateCartItemQuantityCommandHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public Task<UpdateCartItemQuantityResult> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
        {
            return _cartRepository.UpdateCartItemQuantityAsync(
                request.UserId, request.CartItemId, request.Quantity, cancellationToken);
        }
    }
}
