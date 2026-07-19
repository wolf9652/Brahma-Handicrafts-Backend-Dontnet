using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<int> AddUserAsync(Users user);
        Task AddCredentialsAsync(int userId, string passwordHash);
        Task<Users?> GetUserByEmailAsync(string email);
        Task<UsersCredentials?> GetCredentialsByUserIdAsync(int userId);
    }
}