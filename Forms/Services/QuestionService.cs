using Forms.DataAccess;
using Forms.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forms.Services
{
    public class QuestionService : IQuestionService
    {
        public async Task<bool> AddQuestion(QuestionEntity question, FormsDbContext context)
        {
            try
            {
                await context.Questions.AddAsync(question);

                await context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateQuestion(QuestionEntity question, FormsDbContext context)
        {
            try
            {
                await context.Questions
                    .Where(q => q.Id == question.Id)
                    .ExecuteUpdateAsync(s => s
                    .SetProperty(q => q.Title, question.Title)
                    .SetProperty(q => q.Description, question.Description)
                    .SetProperty(q => q.IsVisible, question.IsVisible)
                    .SetProperty(q => q.Options, question.Options)
                    .SetProperty(q => q.Order, question.Order));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteQuestion(int questionId, FormsDbContext context)
        {
            try
            {
                var question = await context.Questions
                    .FirstOrDefaultAsync(q => q.Id == questionId);

                context.Questions
                    .Remove(question);

                await context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
