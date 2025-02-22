using Forms.DataAccess;
using Forms.DataAccess.Entities;

namespace Forms.Services
{
    public interface ITemplateService
    {
        Task<TemplateEntity?> CreateTemplate(string title, string description, int theme, string? imageUrl, string tags, byte isPublic, int createdById, string questions, string? templateAccess, FormsDbContext context);
        Task<TemplateEntity?> GetTemplateById(int templateId, FormsDbContext context);
        Task<bool> RemoveTemplate(int templateId, FormsDbContext context);
        Task<List<TemplateEntity>> SearchTemplates(string searchQuery, FormsDbContext context);
        Task<bool> UpdateQuestion(int templateId, List<QuestionEntity> questions, FormsDbContext context);
        Task<TemplateEntity?> UpdateTemplate(int templateId, string title, string description, int theme, string? imageUrl, string tags, byte isPublic, string questions, string templateAccess, FormsDbContext context);
    }
}