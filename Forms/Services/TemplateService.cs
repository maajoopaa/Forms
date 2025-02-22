using Forms.DataAccess;
using Forms.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using System.Text.Json;

namespace Forms.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly IQuestionService _questionService;

        public TemplateService(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public async Task<TemplateEntity?> GetTemplateById(int templateId, FormsDbContext context)
        {
            var template = await context.Templates
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == templateId);

            return template;
        }

        public async Task<TemplateEntity?> CreateTemplate(string title, string description, int theme, string? imageUrl, string tags, byte isPublic, int createdById,
            string questions, string? templateAccess, FormsDbContext context)
        {
            try
            {
                var tagList = JsonSerializer.Deserialize<List<string>>(tags) ?? new List<string>();

                var questionList = JsonSerializer.Deserialize<List<QuestionEntity>>(questions) ?? new List<QuestionEntity>();

                var templateAccessListString = JsonSerializer.Deserialize<List<string>>(templateAccess ?? "") ?? new List<string>();

                var templateAccessList = new List<int>();

                templateAccessListString.ForEach(tal => templateAccessList.Add(int.Parse(tal)));

                var template = new TemplateEntity
                {
                    Title = title,
                    Description = description,
                    Theme = theme,
                    ImageURL = imageUrl ?? string.Empty,
                    Tags = tagList,
                    IsPublic = isPublic,
                    CreatedById = createdById,
                    Questions = questionList,
                    TemplateAccess = templateAccessList ?? new List<int>(),
                    CreatedAt = DateTime.UtcNow
                };

                await context.Templates
                    .AddAsync(template);

                await context.SaveChangesAsync();

                return template;
            }
            catch
            {
                return null;
            }
        }

        public async Task<TemplateEntity?> UpdateTemplate(int templateId, string title, string description, int theme, string? imageUrl, string tags, byte isPublic, string questions,
            string templateAccess, FormsDbContext context)
        {
            try
            {
                var tagList = JsonSerializer.Deserialize<List<string>>(tags) ?? new List<string>();

                var questionList = JsonSerializer.Deserialize<List<QuestionEntity>>(questions) ?? new List<QuestionEntity>();

                var templateAccessListString = JsonSerializer.Deserialize<List<string>>(templateAccess ?? "") ?? new List<string>();

                var templateAccessList = new List<int>();

                templateAccessListString.ForEach(tal => templateAccessList.Add(int.Parse(tal)));

                await context.Templates
                    .Where(t => t.Id == templateId)
                    .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.Title, title)
                    .SetProperty(t => t.Description, description)
                    .SetProperty(t => t.Theme, theme)
                    .SetProperty(t => t.ImageURL, imageUrl)
                    .SetProperty(t => t.Tags, tagList)
                    .SetProperty(t => t.IsPublic, isPublic)
                    .SetProperty(t => t.TemplateAccess, templateAccessList)
                    );

                await UpdateQuestion(templateId, questionList, context);

                return await GetTemplateById(templateId, context);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> RemoveTemplate(int templateId, FormsDbContext context)
        {
            try
            {
                var template = await context.Templates
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == templateId);

                if (template is not null)
                {
                    context.Templates
                        .Remove(template);

                    await context.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<TemplateEntity>> SearchTemplates(string searchQuery, FormsDbContext context)
        {
            searchQuery = searchQuery.ToLower();

            string query = $"SELECT * FROM \"Templates\" WHERE \"Title\" ILIKE @searchQuery OR \"Description\" ILIKE @searchQuery OR array_to_string(\"Tags\", ' ') ILIKE @searchQuery";
            var templates = await context.Templates
                .FromSqlRaw(query, new NpgsqlParameter("@searchQuery", $"%{searchQuery}%"))
                .ToListAsync();

            return templates;
        }

        public async Task<bool> UpdateQuestion(int templateId,List<QuestionEntity> questions ,FormsDbContext context)
        {
            try
            {
                var template = await GetTemplateById(templateId,context);

                if(template is not null)
                {
                    foreach(var question in questions.Where(q => (int)q.Type == 3))
                    {
                        var options = await context.Options
                            .Where(o => o.QuestionId == question.Id)
                            .ToListAsync();

                        context.Options.RemoveRange(options);
                    }

                    context.Questions.RemoveRange(template.Questions);

                    template.Questions = questions;

                    context.Templates.Update(template);

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
    }
}
