using BRAHMA_HANDICRAFT_BACKEND.Application.Addresses;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : Controller
    {
        private readonly IMediator _mediator;

        public AddressesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/addresses/user/{userId}
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(List<AddressDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAddressByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllAddressesByUserIdQuery(userId), cancellationToken);
            return Ok(result);
        }

        // POST: api/addresses/add
        [HttpPost("add")]
        [ProducesResponseType(typeof(AddAddressResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddAddressByUserId(
            [FromBody] AddAddressRequest request,
            CancellationToken cancellationToken)
        {
            var command = new AddAddressCommand(
                request.UserId, request.CustomerName, request.CustomerPhoneNumber, request.AddressLine1,
                request.City, request.State, request.PostalCode, request.Country, request.IsDefault);
            var address = await _mediator.Send(command, cancellationToken);

            return Ok(new AddAddressResponseDto
            {
                AddressId = address.Id,
                UserId = address.UserId,
                CustomerName = address.CustomerName,
                CustomerPhoneNumber = address.CustomerPhoneNumber
            });
        }

        // PUT: api/addresses/update
        [HttpPut("update")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAddress(
            [FromBody] UpdateAddressRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateAddressCommand(
                request.UserId, request.AddressId, request.CustomerName, request.CustomerPhoneNumber,
                request.AddressLine1, request.City, request.State, request.PostalCode, request.Country,
                request.IsDefault);
            var updated = await _mediator.Send(command, cancellationToken);

            return updated is null ? NotFound(new { message = "Address not found for this user." }) : Ok(updated);
        }

        // POST: api/addresses/remove
        [HttpPost("remove")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveAddressByUserId(
            [FromBody] RemoveAddressRequest request,
            CancellationToken cancellationToken)
        {
            var removed = await _mediator.Send(new RemoveAddressCommand(request.UserId, request.AddressId), cancellationToken);
            return removed ? Ok() : NotFound();
        }
    }
}
