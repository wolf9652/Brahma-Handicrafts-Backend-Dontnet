using BRAHMA_HANDICRAFT_BACKEND.Application.Cart;
using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/cart/add
        [HttpPost("add")]
        [ProducesResponseType(typeof(AddToCartResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddToCartByUserId(
            [FromBody] AddToCartRequest request,
            CancellationToken cancellationToken)
        {
            var command = new AddToCartCommand(request.UserId, request.ProductId, request.DesignId, request.SizeId, request.Quantity);
            var cartItemId = await _mediator.Send(command, cancellationToken);

            return Ok(new AddToCartResponseDto
            {
                CartItemId = cartItemId,
                UserId = request.UserId,
                ProductId = request.ProductId,
                DesignId = request.DesignId,
                SizeId = request.SizeId
            });
        }

        // GET: api/cart/user/{userId}
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(List<CartItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCartItemsByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCartItemsByUserIdQuery(userId), cancellationToken);
            return Ok(result);
        }

        // PUT: api/cart/update
        [HttpPut("update")]
        [ProducesResponseType(typeof(UpdateCartItemQuantityResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCartItemQuantity(
            [FromBody] UpdateCartItemQuantityRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateCartItemQuantityCommand(request.UserId, request.CartItemId, request.Quantity);
            var result = await _mediator.Send(command, cancellationToken);

            return result switch
            {
                UpdateCartItemQuantityResult.NotFound => NotFound(),
                UpdateCartItemQuantityResult.InvalidQuantity => BadRequest(new ErrorResponseDto
                {
                    Message = "Quantity cannot be negative"
                }),
                UpdateCartItemQuantityResult.Deleted => Ok(new UpdateCartItemQuantityResponseDto { CartItemId = request.CartItemId, Quantity = 0 }),
                UpdateCartItemQuantityResult.Updated => Ok(new UpdateCartItemQuantityResponseDto { CartItemId = request.CartItemId, Quantity = request.Quantity }),
                _ => StatusCode(500)
            };
        }

        // DELETE: api/cart/{cartItemId}?userId={userId}
        [HttpDelete("{cartItemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCartItem(
            Guid cartItemId,
            [FromQuery] Guid userId,
            CancellationToken cancellationToken)
        {
            var deleted = await _mediator.Send(new DeleteCartItemCommand(userId, cartItemId), cancellationToken);

            if (!deleted)
            {
                return NotFound(new ErrorResponseDto { Message = "Cart item not found" });
            }

            return Ok();
        }

        // DELETE: api/cart/clear?userId={userId}
        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearCart(
            [FromQuery] Guid userId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new ClearCartCommand(userId), cancellationToken);
            return Ok();
        }
    }
}
