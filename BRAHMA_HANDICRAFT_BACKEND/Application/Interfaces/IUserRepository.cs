using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<Guid> AddUserAsync(Users user);
        Task<Users?> GetUserByEmailAsync(string email);
    }
}
