using BRAHMA_HANDICRAFT_BACKEND.Application.Admin;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/promotions")]
    [Authorize(Roles = "Admin")]
    public class AdminPromotionsController : Controller
    {
        private readonly IMediator _mediator;

        public AdminPromotionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/admin/promotions
        [HttpPost]
        [ProducesResponseType(typeof(CreatePromotionResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePromotion(
            [FromBody] CreatePromotionRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreatePromotionCommand(
                request.Name, request.DiscountType, request.DiscountValue,
                request.StartDate, request.EndDate, request.IsActive, request.ProductIds);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        // GET: api/admin/promotions
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<AdminPromotionListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPromotions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAllPromotionsQuery(pageNumber, pageSize), cancellationToken);
            return Ok(result);
        }

        // GET: api/admin/promotions/{promotionId}
        [HttpGet("{promotionId}")]
        [ProducesResponseType(typeof(AdminPromotionDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPromotionById(Guid promotionId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPromotionByIdQuery(promotionId), cancellationToken);
            return result is null ? NotFound(new { message = "Promotion not found." }) : Ok(result);
        }

        // PUT: api/admin/promotions/{promotionId}
        [HttpPut("{promotionId}")]
        [ProducesResponseType(typeof(CreatePromotionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePromotion(
            Guid promotionId,
            [FromBody] UpdatePromotionRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdatePromotionCommand(
                promotionId, request.Name, request.DiscountType, request.DiscountValue,
                request.StartDate, request.EndDate);
            var result = await _mediator.Send(command, cancellationToken);
            return result is null ? NotFound(new { message = "Promotion not found." }) : Ok(result);
        }

        // PATCH: api/admin/promotions/{promotionId}/status
        [HttpPatch("{promotionId}/status")]
        [ProducesResponseType(typeof(UpdatePromotionStatusResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePromotionStatus(
            Guid promotionId,
            [FromBody] UpdatePromotionStatusRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdatePromotionStatusCommand(promotionId, request.IsActive), cancellationToken);
            return result is null ? NotFound(new { message = "Promotion not found." }) : Ok(result);
        }

        // DELETE: api/admin/promotions/{promotionId}
        [HttpDelete("{promotionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePromotion(Guid promotionId, CancellationToken cancellationToken)
        {
            var deleted = await _mediator.Send(new DeletePromotionCommand(promotionId), cancellationToken);
            return deleted ? Ok() : NotFound(new { message = "Promotion not found." });
        }

        // POST: api/admin/promotions/{promotionId}/banners
        [HttpPost("{promotionId}/banners")]
        [ProducesResponseType(typeof(AddPromotionBannerResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddPromotionBanner(
            Guid promotionId,
            [FromBody] AddPromotionBannerRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new AddPromotionBannerCommand(promotionId, request.ImageUrl, request.AltText), cancellationToken);
            return result is null ? NotFound(new { message = "Promotion not found." }) : Ok(result);
        }

        // DELETE: api/admin/promotions/{promotionId}/banners/{bannerId}
        [HttpDelete("{promotionId}/banners/{bannerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemovePromotionBanner(Guid promotionId, Guid bannerId, CancellationToken cancellationToken)
        {
            var removed = await _mediator.Send(new RemovePromotionBannerCommand(promotionId, bannerId), cancellationToken);
            return removed ? Ok() : NotFound(new { message = "Banner not found for this promotion." });
        }
    }
}
