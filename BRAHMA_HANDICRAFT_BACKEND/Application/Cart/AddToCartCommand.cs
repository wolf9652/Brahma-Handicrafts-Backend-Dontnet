using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Cart
{
    public record AddToCartCommand(Guid UserId, Guid ProductId, Guid DesignId, Guid SizeId, int Quantity)
        : IRequest<Guid>;

    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Guid>
    {
        private readonly ICartRepository _cartRepository;

        public AddToCartCommandHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public Task<Guid> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            return _cartRepository.AddToCartAsync(
                request.UserId, request.ProductId, request.DesignId, request.SizeId, request.Quantity, cancellationToken);
        }
    }
}
