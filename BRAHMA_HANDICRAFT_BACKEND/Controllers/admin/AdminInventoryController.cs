using BRAHMA_HANDICRAFT_BACKEND.Application.Admin;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/inventory")]
    [Authorize(Roles = "Admin")]
    public class AdminInventoryController : Controller
    {
        private readonly IMediator _mediator;

        public AdminInventoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/admin/inventory
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<InventoryItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllInventory(
            [FromQuery] bool includeInactive,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllInventoryQuery(includeInactive, pageNumber, pageSize), cancellationToken);
            return Ok(result);
        }

        // PUT: api/admin/inventory/{inventoryStockId}
        [HttpPut("{inventoryStockId}")]
        [ProducesResponseType(typeof(InventoryItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateQuantity(
            Guid inventoryStockId,
            [FromBody] UpdateInventoryQuantityRequest request,
            CancellationToken cancellationToken)
        {
            var updated = await _mediator.Send(new UpdateInventoryQuantityCommand(inventoryStockId, request.Quantity), cancellationToken);
            return updated is null ? NotFound(new { message = "Inventory stock item not found." }) : Ok(updated);
        }
    }
}
