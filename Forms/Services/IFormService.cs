using Forms.DataAccess;
using Forms.DataAccess.Entities;

namespace Forms.Services
{
    public interface IFormService
    {
        Task<FormEntity?> FillForm(int templateId, int filledById, string answers, FormsDbContext context);
        Task<FormEntity?> GetFormById(int formId, FormsDbContext context);
        Task<bool> RemoveForm(int formId, FormsDbContext context);
        Task<FormEntity?> UpdateForm(int formId, string answers, FormsDbContext context);
        Task<bool> UpdateAnswers(int formId, List<AnswerEntity> answers, FormsDbContext context);
    }
}