using Forms.DataAccess;
using Forms.DataAccess.Entities;

namespace Forms.Services
{
    public interface ICommentService
    {
        Task<CommentEntity?> AddComment(string text, int templateId, int createdById, FormsDbContext context);
        Task<bool> RemoveComment(int commentId, FormsDbContext context);
    }
}