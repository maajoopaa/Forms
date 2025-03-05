using Forms.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forms.Controllers
{
    [ApiController]
    [Route("api/survey")]
    public class SurveyController : Controller
    {
        private readonly FormsDbContext _context;

        public SurveyController(FormsDbContext context)
        {
            _context = context;
        }

        [HttpGet("templates")]
        [Authorize(AuthenticationSchemes = "ApiToken")]
        public IActionResult GetTemplates()
        {
            var user = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.ApiToken == GetApiToken());

            if (user == null) return Unauthorized();

            return Ok(user.Templates.Select(t => new
            {
                Id = t.Id,
                Title = t.Title
            }));
        }

        [HttpGet("templates/{id}/aggregated")]
        [Authorize(AuthenticationSchemes = "ApiToken")]
        public IActionResult GetAggregatedData(int id)
        {
            var template = _context.Templates
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == id && t.CreatedBy.ApiToken == GetApiToken());

            if (template == null) return NotFound();

            var result = new
            {
                Author = template.CreatedBy.Username,
                Title = template.Title,
                Questions = template.Questions.Select(q => new
                {
                    Text = q.Title,
                    Type = q.Type,
                    Average = (int)q.Type == 2 ? q.Answers.Average(a => Convert.ToInt32(a.Value)) : 0,
                    Min = (int)q.Type == 2 ? q.Answers.Min(a => Convert.ToInt32(a.Value)) : 0,
                    Max = (int)q.Type == 2 ? q.Answers.Max(a => Convert.ToInt32(a.Value)) : 0,
                    TopAnswers = (int)q.Type == 1 || (int)q.Type == 2 ?
                        q.Answers
                            .GroupBy(a => a.Value)
                            .OrderByDescending(g => g.Count())
                            .Take(3)
                            .Select(g => g.Key)
                        : null
                })
            };

            return Ok(result);
        }

        private string? GetApiToken()
        {
            return Request.Headers["X-API-TOKEN"];
        }
    }
}
