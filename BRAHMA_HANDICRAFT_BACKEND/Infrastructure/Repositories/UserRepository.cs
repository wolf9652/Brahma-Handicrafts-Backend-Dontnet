using BRAHMA_HANDICRAFT_BACKEND.Data;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddUserAsync(Users user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.EmailId == email);
        }

        public async Task<(UpdateUserResult Result, Users? User)> UpdateUserProfileAsync(
            Guid userId,
            string? firstName,
            string? lastName,
            string? phoneNumber,
            string? emailId,
            Gender? gender,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
            {
                return (UpdateUserResult.NotFound, null);
            }

            if (emailId is not null)
            {
                var emailInUse = await _context.Users.AnyAsync(
                    u => u.Id != userId && u.EmailId == emailId, cancellationToken);

                if (emailInUse)
                {
                    return (UpdateUserResult.EmailConflict, null);
                }

                user.EmailId = emailId;
            }

            if (firstName is not null)
            {
                user.FirstName = firstName;
            }

            if (lastName is not null)
            {
                user.LastName = lastName;
            }

            if (phoneNumber is not null)
            {
                user.PhoneNumber = phoneNumber;
            }

            if (gender is not null)
            {
                user.Gender = gender;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return (UpdateUserResult.Success, user);
        }

        public async Task<(List<AdminUserDto> Items, int TotalCount)> GetAllUsersAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.Role)
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    EmailId = u.EmailId,
                    PhoneNumber = u.PhoneNumber,
                    Gender = u.Gender == null ? null : u.Gender.ToString(),
                    OrdersCount = _context.Orders.Count(o => o.UserId == u.Id)
                })
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
