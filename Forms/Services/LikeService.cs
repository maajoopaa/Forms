using Forms.DataAccess.Entities;
using Forms.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Forms.Services
{
    public class LikeService : ILikeService
    {
        public async Task<LikeEntity?> LikeTemplate(int templateId, int likedById, FormsDbContext context)
        {
            try
            {
                var like = new LikeEntity
                {
                    TemplateId = templateId,
                    LikedById = likedById,
                    LikedAt = DateTime.UtcNow
                };

                await context.Likes
                    .AddAsync(like);

                await context.SaveChangesAsync();

                return like;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UnlikeTemplate(int likeId, FormsDbContext context)
        {
            try
            {
                var like = await context.Likes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Id == likeId);

                if (like is not null)
                {
                    context.Likes
                        .Remove(like);

                    context.SaveChanges();

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
