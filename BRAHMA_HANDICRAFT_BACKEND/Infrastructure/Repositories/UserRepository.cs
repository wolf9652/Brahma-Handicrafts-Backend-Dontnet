using BRAHMA_HANDICRAFT_BACKEND.Data;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddUserAsync(Users user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.UserId;
        }

        public async Task AddCredentialsAsync(int userId, string passwordHash)
        {
            var credentials = new UsersCredentials
            {
                UserId = userId,
                PasswordHash = passwordHash
            };
            _context.UsersCredentials.Add(credentials);
            await _context.SaveChangesAsync();
        }

        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UsersCredentials?> GetCredentialsByUserIdAsync(int userId)
        {
            return await _context.UsersCredentials.FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
