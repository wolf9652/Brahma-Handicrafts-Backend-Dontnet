using BRAHMA_HANDICRAFT_BACKEND.Application.Admin;
using BRAHMA_HANDICRAFT_BACKEND.Application.Designs;
using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Roles = "Admin")]
    public class AdminProductsController : Controller
    {
        private readonly IMediator _mediator;

        public AdminProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/admin/products
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<AdminProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] bool includeInactive = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAdminProductsQuery(includeInactive, pageNumber, pageSize), cancellationToken);
            return Ok(result);
        }

        // GET: api/admin/products/{productId}
        [HttpGet("{productId}")]
        [ProducesResponseType(typeof(CreatedProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductDetails(Guid productId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetProductDetailsQuery(productId), cancellationToken);
            return result is null ? NotFound(new { message = "Product not found." }) : Ok(result);
        }

        // POST: api/admin/products
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(CreatedProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request, CancellationToken cancellationToken)
        {
            var (result, product) = await _mediator.Send(new CreateProductCommand(request), cancellationToken);

            return result switch
            {
                CreateProductResult.NoDesigns => BadRequest(new { message = "Product must have at least one design." }),
                CreateProductResult.DesignWithNoSizes => BadRequest(new { message = "Every design must have at least one size." }),
                _ => Ok(product)
            };
        }

        // PUT: api/admin/products/{productId}
        [HttpPut("{productId}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(CreatedProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateProduct(
            Guid productId,
            [FromForm] UpdateProductRequest request,
            CancellationToken cancellationToken)
        {
            if (productId == Guid.Empty)
            {
                return BadRequest(new { message = "productId is required." });
            }

            var (result, product, errorMessage) = await _mediator.Send(new UpdateProductCommand(productId, request), cancellationToken);

            return result switch
            {
                UpdateProductResult.ProductNotFound => NotFound(new { message = "Product not found." }),
                UpdateProductResult.InvalidReference => BadRequest(new { message = errorMessage }),
                UpdateProductResult.WouldHaveNoDesigns => BadRequest(new { message = errorMessage }),
                UpdateProductResult.DesignWouldHaveNoSizes => BadRequest(new { message = errorMessage }),
                UpdateProductResult.ReferencedElsewhere => Conflict(new { message = errorMessage }),
                _ => Ok(product)
            };
        }
    }
}
