using BRAHMA_HANDICRAFT_BACKEND.Application.Models;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public interface IDesignRepository
    {
        Task<DesignDetailsDto?> GetDesignDetailsByIdAsync(Guid productId, Guid designId, CancellationToken cancellationToken);
    }
}
