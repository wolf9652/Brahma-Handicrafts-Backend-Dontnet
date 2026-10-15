using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record UpdateInventoryQuantityCommand(Guid InventoryStockId, int Quantity)
        : IRequest<InventoryItemDto?>;

    public class UpdateInventoryQuantityCommandHandler : IRequestHandler<UpdateInventoryQuantityCommand, InventoryItemDto?>
    {
        private readonly IInventoryRepository _inventoryRepository;

        public UpdateInventoryQuantityCommandHandler(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public Task<InventoryItemDto?> Handle(UpdateInventoryQuantityCommand request, CancellationToken cancellationToken)
        {
            return _inventoryRepository.UpdateQuantityAsync(request.InventoryStockId, request.Quantity, cancellationToken);
        }
    }
}
