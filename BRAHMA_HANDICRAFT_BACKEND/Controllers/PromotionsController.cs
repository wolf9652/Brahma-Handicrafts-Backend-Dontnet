using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Application.Promotions;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionsController : Controller
    {
        private readonly IMediator _mediator;

        public PromotionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/promotions/active
        [HttpGet("active")]
        [ProducesResponseType(typeof(List<CustomerPromotionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivePromotions(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetActivePromotionsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
