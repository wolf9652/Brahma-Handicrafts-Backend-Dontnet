using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Application.Wishlist;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : Controller
    {
        private readonly IMediator _mediator;

        public WishlistController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/wishlist/add
        [HttpPost("add")]
        [ProducesResponseType(typeof(AddToWishlistResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddToWishlistByUserId(
            [FromBody] AddToWishlistRequest request,
            CancellationToken cancellationToken)
        {
            var command = new AddToWishlistCommand(request.UserId, request.ProductId, request.DesignId);
            var wishlistItemId = await _mediator.Send(command, cancellationToken);

            return Ok(new AddToWishlistResponseDto
            {
                WishlistItemId = wishlistItemId,
                UserId = request.UserId,
                ProductId = request.ProductId,
                DesignId = request.DesignId
            });
        }

        // GET: api/wishlist/user/{userId}
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(List<WishlistItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWishlistedItemsByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetWishlistedItemsByUserIdQuery(userId), cancellationToken);
            return Ok(result);
        }

        // DELETE: api/wishlist/{wishlistItemId}?userId={userId}
        [HttpDelete("{wishlistItemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFromWishlist(
            Guid wishlistItemId,
            [FromQuery] Guid userId,
            CancellationToken cancellationToken)
        {
            var deleted = await _mediator.Send(new DeleteFromWishlistCommand(userId, wishlistItemId), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}
