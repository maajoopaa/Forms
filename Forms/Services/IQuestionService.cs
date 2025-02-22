using Forms.DataAccess;
using Forms.DataAccess.Entities;

namespace Forms.Services
{
    public interface IQuestionService
    {
        Task<bool> AddQuestion(QuestionEntity question, FormsDbContext context);
        Task<bool> DeleteQuestion(int questionId, FormsDbContext context);
        Task<bool> UpdateQuestion(QuestionEntity question, FormsDbContext context);
    }
}