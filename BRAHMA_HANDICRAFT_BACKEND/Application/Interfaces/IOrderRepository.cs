using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public enum CreateOrderResult
    {
        Success,
        UserNotFound,
        ShippingAddressNotFound,
        BillingAddressNotFound,
        NoItems,
        InvalidItem,
        InsufficientStock
    }

    public enum UpdateOrderStatusResult
    {
        Success,
        OrderNotFound,
        InvalidTransition
    }

    public interface IOrderRepository
    {
        Task<(CreateOrderResult Result, OrderDto? Order, string? ErrorMessage)> CreateOrderAsync(
            CreateOrderRequest request,
            CancellationToken cancellationToken);

        Task<(List<OrderDto> Items, int TotalCount)> GetOrdersByUserIdAsync(
            Guid userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<(UpdateOrderStatusResult Result, Guid OrderId, OrderStatus Status, string? ErrorMessage)> UpdateOrderStatusAsync(
            Guid orderId,
            OrderStatus status,
            CancellationToken cancellationToken);

        Task<(List<AdminOrderDto> Items, int TotalCount)> GetAllOrdersAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
