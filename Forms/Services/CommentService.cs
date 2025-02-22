using Forms.DataAccess;
using Forms.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forms.Services
{
    public class CommentService : ICommentService
    {
        public async Task<CommentEntity?> AddComment(string text, int templateId, int createdById, FormsDbContext context)
        {
            try
            {
                var comment = new CommentEntity
                {
                    Text = text,
                    TemplateId = templateId,
                    CreatedById = createdById,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Comments
                    .AddAsync(comment);

                await context.SaveChangesAsync();

                return comment;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> RemoveComment(int commentId, FormsDbContext context)
        {
            try
            {
                var comment = await context.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == commentId);

                if (comment is not null)
                {
                    context.Comments
                        .Remove(comment);

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
