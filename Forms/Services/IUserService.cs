using Forms.DataAccess;
using Forms.DataAccess.Entities;

namespace Forms.Services
{
    public interface IUserService
    {
        Task<bool> BlockUser(int userId, FormsDbContext context);
        Task<UserEntity?> GetUser(int userId, FormsDbContext context);
        string HashPassword(string password);
        Task<bool> PromoteToAdmin(int userId, FormsDbContext context);
        Task<bool> RemoveFromAdmin(int userId, FormsDbContext context);
        Task<bool> RemoveUser(int userId, FormsDbContext context);
        Task<bool> UnblockUser(int userId, FormsDbContext context);
        Task<bool> UpdateUser(UserEntity newUser, FormsDbContext context);
        bool VertifyPassword(string password, string passwordHash);
        Task<UserSettingEntity> GetSettings(int userId, FormsDbContext context);
        Task<bool> UserIsBlocked(int userId);
    }
}