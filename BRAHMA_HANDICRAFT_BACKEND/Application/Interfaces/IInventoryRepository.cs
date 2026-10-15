using BRAHMA_HANDICRAFT_BACKEND.Application.Models;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public interface IInventoryRepository
    {
        Task<(List<InventoryItemDto> Items, int TotalCount)> GetAllInventoryAsync(
            bool includeInactive,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<InventoryItemDto?> UpdateQuantityAsync(
            Guid inventoryStockId,
            int quantity,
            CancellationToken cancellationToken);
    }
}
