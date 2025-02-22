using BCrypt.Net;
using Forms.DataAccess;
using Forms.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace Forms.Services
{
    public class UserService : IUserService
    {
        private readonly FormsDbContext _context;

        public UserService(FormsDbContext context)
        {
            _context = context;
        }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VertifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public async Task<UserEntity?> GetUser(int userId, FormsDbContext context)
        {
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user;
        }

        public async Task<bool> UpdateUser(UserEntity newUser, FormsDbContext context)
        {
            try
            {
                context.Users
                    .Update(newUser);

                await context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveUser(int userId, FormsDbContext context)
        {

            try
            {
                await context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == userId)
                    .ExecuteDeleteAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BlockUser(int userId, FormsDbContext context)
        {
            try
            {
                await context.Users
                        .AsNoTracking()
                        .Where(u => u.Id == userId)
                        .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.IsBlocked, 1));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UnblockUser(int userId, FormsDbContext context)
        {
            try
            {
                await context.Users
                        .AsNoTracking()
                        .Where(u => u.Id == userId)
                        .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.IsBlocked, 0));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> PromoteToAdmin(int userId, FormsDbContext context)
        {
            try
            {
                await context.Users
                        .AsNoTracking()
                        .Where(u => u.Id == userId)
                        .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.IsAdmin, 1));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromAdmin(int userId, FormsDbContext context)
        {
            try
            {
                await context.Users
                        .AsNoTracking()
                        .Where(u => u.Id == userId)
                        .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.IsAdmin, 0));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserSettingEntity> GetSettings(int userId, FormsDbContext context)
        {
            var settings = await context.UserSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == userId);

            return settings;
        }

        public async Task<bool> UserIsBlocked(int userId)
        {
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                return user.IsBlocked == 1;
            }
            catch
            {
                return false;
            }
        }
    }
}
