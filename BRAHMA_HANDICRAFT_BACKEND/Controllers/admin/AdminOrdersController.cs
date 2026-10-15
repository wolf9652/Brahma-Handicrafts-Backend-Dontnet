using BRAHMA_HANDICRAFT_BACKEND.Application.Admin;
using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Application.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly IMediator _mediator;

        public AdminOrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // PATCH: api/admin/orders/{orderId}/status
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

        // GET: api/admin/orders
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<AdminOrderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAllOrdersQuery(pageNumber, pageSize), cancellationToken);
            return Ok(result);
        }
    }
}
