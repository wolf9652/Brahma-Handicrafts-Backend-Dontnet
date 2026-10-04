using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public enum UpdateUserResult
    {
        NotFound,
        EmailConflict,
        Success
    }

    public interface IUserRepository
    {
        Task<Guid> AddUserAsync(Users user);
        Task<Users?> GetUserByEmailAsync(string email);

        Task<(UpdateUserResult Result, Users? User)> UpdateUserProfileAsync(
            Guid userId,
            string? firstName,
            string? lastName,
            string? phoneNumber,
            string? emailId,
            Gender? gender,
            CancellationToken cancellationToken);

        Task<(List<AdminUserDto> Items, int TotalCount)> GetAllUsersAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<AdminUserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
