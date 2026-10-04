using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Application.Promotions;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers
{
    [ApiController]
    [Route("api/promotions")]
    public class PromotionController : Controller
    {
        private readonly IMediator _mediator;

        public PromotionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/promotions/discounted-products
        [HttpGet("discounted-products")]
        [ProducesResponseType(typeof(PaginatedResponse<DiscountedProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDiscountedProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDiscountedProductsQuery(pageNumber, pageSize), cancellationToken);
            return Ok(result);
        }

        // GET: api/promotions/banners
        [HttpGet("banners")]
        [ProducesResponseType(typeof(List<PublicPromotionBannerDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBanners(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetActivePromotionBannersQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
