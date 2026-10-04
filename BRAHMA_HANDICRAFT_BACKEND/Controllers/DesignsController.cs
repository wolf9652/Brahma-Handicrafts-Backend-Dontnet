using BRAHMA_HANDICRAFT_BACKEND.Application.Designs;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DesignsController : Controller
    {
        private readonly IMediator _mediator;

        public DesignsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/designs/{productId}/{designId}
        [HttpGet("{productId}/{designId}")]
        [ProducesResponseType(typeof(DesignDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDesignDetailsByDesignId(Guid productId, Guid designId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetDesignDetailsByDesignIdQuery(productId, designId), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
    }
}
