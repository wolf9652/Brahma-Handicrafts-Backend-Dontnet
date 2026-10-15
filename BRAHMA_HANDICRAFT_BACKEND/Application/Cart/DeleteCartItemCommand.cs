using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Cart
{
    public record DeleteCartItemCommand(Guid UserId, Guid CartItemId)
        : IRequest<bool>;

    public class DeleteCartItemCommandHandler : IRequestHandler<DeleteCartItemCommand, bool>
    {
        private readonly ICartRepository _cartRepository;

        public DeleteCartItemCommandHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public Task<bool> Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
        {
            return _cartRepository.DeleteCartItemAsync(request.UserId, request.CartItemId, cancellationToken);
        }
    }
}
