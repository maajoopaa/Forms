using Forms.DataAccess;
using Forms.DataAccess.Entities;
using Forms.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace Forms.Controllers
{
    public class CommentController : Controller
    {
        private readonly FormsDbContext _context;
        private readonly ICommentService _commentService;
        private readonly ITemplateService _templateService;
        private readonly IUserService _userService;

        public CommentController(FormsDbContext context, ICommentService commentService, ITemplateService templateService, IUserService userService)
        {
            _context = context;
            _commentService = commentService;
            _templateService = templateService;
            _userService = userService;
        }

        [Authorize]
        public async Task<IActionResult> Add(int templateId, string text)
        {
            if(text is not null)
            {
                var user = await GetUserFromCookies();

                var userId = user.Id;

                var comment = await _commentService.AddComment(text, templateId,userId, _context);

                if (comment is null)
                {
                    ViewBag.Error = "Во время сохранения произошла ошибка, пожалуйста перепроверьте данные!";
                }
            }

            return Redirect($"/Template/View?templateId={templateId}");
        }

        [Authorize]
        public async Task<IActionResult> Remove(int commentId)
        {
            var isCorrect = await _commentService.RemoveComment(commentId, _context);

            if (!isCorrect)
            {
                ViewBag.Error = "Во время удаления произошла ошибка, попробуйте еще раз!";

                return View();
            }

            return RedirectToAction();//Страница просмотра шаблона
        }
        
        public async Task<IActionResult> GetList(int templateId)
        {
            var comments = await _context.Comments
                .AsNoTracking()
                .Where(c => c.TemplateId == templateId)
                .ToListAsync();

            return PartialView("ViewList", comments);
        }

        private async Task<UserEntity> GetUserFromCookies()
        {
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userId = int.Parse(claimValue);

            var user = await _userService.GetUser(userId, _context);

            return user;
        }
    }
}
