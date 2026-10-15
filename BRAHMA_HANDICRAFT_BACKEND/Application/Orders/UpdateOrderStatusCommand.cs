using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Orders
{
    public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status)
        : IRequest<(UpdateOrderStatusResult Result, Guid OrderId, OrderStatus Status, string? ErrorMessage)>;

    public class UpdateOrderStatusCommandHandler
        : IRequestHandler<UpdateOrderStatusCommand, (UpdateOrderStatusResult Result, Guid OrderId, OrderStatus Status, string? ErrorMessage)>
    {
        private readonly IOrderRepository _orderRepository;

        public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task<(UpdateOrderStatusResult Result, Guid OrderId, OrderStatus Status, string? ErrorMessage)> Handle(
            UpdateOrderStatusCommand command, CancellationToken cancellationToken)
        {
            return _orderRepository.UpdateOrderStatusAsync(command.OrderId, command.Status, cancellationToken);
        }
    }
}
