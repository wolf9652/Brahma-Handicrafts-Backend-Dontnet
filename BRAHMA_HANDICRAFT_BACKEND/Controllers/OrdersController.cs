using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Application.Orders;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/orders
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var (result, order, errorMessage) = await _mediator.Send(new CreateOrderCommand(request), cancellationToken);

            return result switch
            {
                CreateOrderResult.UserNotFound => NotFound(new { message = errorMessage }),
                CreateOrderResult.ShippingAddressNotFound => BadRequest(new { message = errorMessage }),
                CreateOrderResult.BillingAddressNotFound => BadRequest(new { message = errorMessage }),
                CreateOrderResult.NoItems => BadRequest(new { message = errorMessage }),
                CreateOrderResult.InvalidItem => BadRequest(new { message = errorMessage }),
                CreateOrderResult.InsufficientStock => Conflict(new { message = errorMessage }),
                _ => Ok(order)
            };
        }

        // GET: api/orders/user/{userId}
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(PaginatedResponse<OrderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrdersByUserId(
            Guid userId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetOrdersByUserIdQuery(userId, pageNumber, pageSize), cancellationToken);
            return Ok(result);
        }

        // PATCH: api/orders/{orderId}/status
        [HttpPatch("{orderId}/status")]
        [ProducesResponseType(typeof(UpdateOrderStatusResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateOrderStatus(
            Guid orderId,
            [FromBody] UpdateOrderStatusRequest request,
            CancellationToken cancellationToken)
        {
            var (result, resultOrderId, status, errorMessage) = await _mediator.Send(
                new UpdateOrderStatusCommand(orderId, request.Status), cancellationToken);

            return result switch
            {
                UpdateOrderStatusResult.OrderNotFound => NotFound(new { message = errorMessage }),
                UpdateOrderStatusResult.InvalidTransition => Conflict(new { message = errorMessage }),
                _ => Ok(new UpdateOrderStatusResponseDto { OrderId = resultOrderId, OrderStatus = status })
            };
        }
    }
}
