using BRAHMA_HANDICRAFT_BACKEND.Application.Admin;
using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
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
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(PromotionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePromotion(
            [FromForm] CreatePromotionRequest request,
            CancellationToken cancellationToken)
        {
            (CreatePromotionResult Result, PromotionDto? Promotion, List<Guid>? InvalidProductIds) outcome;

            try
            {
                outcome = await _mediator.Send(new CreatePromotionCommand(request), cancellationToken);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            if (outcome.Result == CreatePromotionResult.InvalidProductIds)
            {
                return BadRequest(new
                {
                    message = "One or more productIds do not exist.",
                    invalidProductIds = outcome.InvalidProductIds
                });
            }

            return Ok(outcome.Promotion);
        }

        // GET: api/admin/promotions
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<AdminPromotionListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPromotions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAllPromotionsQuery(pageNumber, pageSize), cancellationToken);
            return Ok(result);
        }

        // PUT: api/admin/promotions/{promotionId}
        [HttpPut("{promotionId}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(PromotionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePromotion(
            Guid promotionId,
            [FromForm] UpdatePromotionRequest request,
            CancellationToken cancellationToken)
        {
            (UpdatePromotionResult Result, PromotionDto? Promotion, List<Guid>? InvalidProductIds) outcome;

            try
            {
                outcome = await _mediator.Send(new UpdatePromotionCommand(promotionId, request), cancellationToken);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return outcome.Result switch
            {
                UpdatePromotionResult.PromotionNotFound => NotFound(new { message = "Promotion not found." }),
                UpdatePromotionResult.InvalidProductIds => BadRequest(new
                {
                    message = "One or more productIds do not exist.",
                    invalidProductIds = outcome.InvalidProductIds
                }),
                _ => Ok(outcome.Promotion)
            };
        }

        // PATCH: api/admin/promotions/{promotionId}/status
        [HttpPatch("{promotionId}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TogglePromotionActive(Guid promotionId, CancellationToken cancellationToken)
        {
            var (result, isActive) = await _mediator.Send(new TogglePromotionActiveCommand(promotionId), cancellationToken);

            return result switch
            {
                TogglePromotionActiveResult.PromotionNotFound => NotFound(new { message = "Promotion not found." }),
                _ => Ok(new { promotionId, isActive })
            };
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
    }
}
