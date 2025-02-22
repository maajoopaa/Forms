using Forms.DataAccess;
using Forms.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Forms.Services
{
    public class FormService : IFormService
    {
        public async Task<FormEntity?> FillForm(int templateId, int filledById, string answers, FormsDbContext context)
        {
            try
            {
                var answersDictionary = JsonSerializer.Deserialize<Dictionary<int, string>>(answers);

                var answerList = new List<AnswerEntity>();

                if(answersDictionary is not null)
                {
                    foreach (var answer in answersDictionary)
                    {
                        answerList.Add(new AnswerEntity
                        {
                            QuestionId = answer.Key,
                            Value = answer.Value
                        });
                    }

                    var form = new FormEntity
                    {
                        TemplateId = templateId,
                        FilledById = filledById,
                        FilledAt = DateTime.UtcNow,
                        Answers = answerList
                    };

                    await context.Forms
                        .AddAsync(form);

                    await context.SaveChangesAsync();

                    return form;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<FormEntity?> GetFormById(int formId, FormsDbContext context)
        {
            try
            {
                var form = await context.Forms
                    .FirstOrDefaultAsync(f => f.Id == formId);

                return form;
            }
            catch
            {
                return null;
            }
        }

        public async Task<FormEntity?> UpdateForm(int formId, string answers, FormsDbContext context)
        {
            try
            {
                var answersDictionary = JsonSerializer.Deserialize<Dictionary<int, string>>(answers);

                var answerList = new List<AnswerEntity>();

                if (answersDictionary is not null)
                {
                    foreach (var answer in answersDictionary)
                    {
                        answerList.Add(new AnswerEntity
                        {
                            QuestionId = answer.Key,
                            Value = answer.Value,
                            FormId = formId
                        });
                    }

                    await UpdateAnswers(formId, answerList, context);

                    return await GetFormById(formId, context);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> RemoveForm(int formId, FormsDbContext context)
        {
            try
            {
                var form = await GetFormById(formId, context);

                if (form is not null)
                {
                    context.Forms
                        .Remove(form);

                    await context.SaveChangesAsync();

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAnswers(int formId,List<AnswerEntity> answers, FormsDbContext context)
        {
            var form = await GetFormById(formId, context);

            if(form is not null)
            {
                context.Answers.RemoveRange(form.Answers);

                await context.Answers.AddRangeAsync(answers);

                await context.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
