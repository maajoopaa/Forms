using Forms.DataAccess;
using Forms.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace Forms.Controllers
{
    public class LikeController : Controller
    {
        private readonly FormsDbContext _context;
        private readonly ILikeService _likeService;

        public LikeController(FormsDbContext context, ILikeService likeService)
        {
            _context = context;
            _likeService = likeService;
        }

        [Authorize]
        public async Task<IActionResult> Add(int templateId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return StatusCode(403);
            }

            var userId = GetUserFromCookies();

            var like = await _likeService.LikeTemplate(templateId, userId, _context);

            if (like is null)
            {
                ViewBag.Error = "Во время сохранения произошла ошибка, попробуйте еще раз!";

                return StatusCode(404);
            }

            return StatusCode(200);
        }

        [Authorize]
        public async Task<IActionResult> Remove(int templateId, int userId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return StatusCode(403);
            }

            var like = await _context.Likes
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.TemplateId == templateId && l.LikedById == userId);

            var isCorrect = await _likeService.UnlikeTemplate(like.Id, _context);

            if (!isCorrect)
            {
                ViewBag.Error = "Во время сохранения произошла ошибка, попробуйте еще раз!";

                return StatusCode(404);
            }

            return StatusCode(200);
        }

        public async Task<IActionResult> Count(int templateId)
        {
            var template = await _context.Templates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == templateId);

            var count = template.Likes.Count;

            return Content(count.ToString());
        }

        private int GetUserFromCookies()
        {
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userId = int.Parse(claimValue);

            return userId;
        }
    }
}
