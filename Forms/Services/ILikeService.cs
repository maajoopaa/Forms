using Forms.DataAccess;
using Forms.DataAccess.Entities;

namespace Forms.Services
{
    public interface ILikeService
    {
        Task<LikeEntity?> LikeTemplate(int templateId, int likedById, FormsDbContext context);
        Task<bool> UnlikeTemplate(int likeId, FormsDbContext context);
    }
}