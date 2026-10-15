using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Orders
{
    public record CreateOrderCommand(CreateOrderRequest Request)
        : IRequest<(CreateOrderResult Result, OrderDto? Order, string? ErrorMessage)>;

    public class CreateOrderCommandHandler
        : IRequestHandler<CreateOrderCommand, (CreateOrderResult Result, OrderDto? Order, string? ErrorMessage)>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task<(CreateOrderResult Result, OrderDto? Order, string? ErrorMessage)> Handle(
            CreateOrderCommand command, CancellationToken cancellationToken)
        {
            return _orderRepository.CreateOrderAsync(command.Request, cancellationToken);
        }
    }
}
