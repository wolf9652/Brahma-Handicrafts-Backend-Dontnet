using BRAHMA_HANDICRAFT_BACKEND.Application.Products;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/products
        // includeInactive=true is intended for the admin page; there is no authentication/authorization
        // system in this project yet, so this parameter is currently open to any caller.
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] bool includeInactive = false,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllProductsQuery(includeInactive, pageNumber, pageSize);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
