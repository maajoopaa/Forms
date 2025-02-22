using Forms.DataAccess;
using Forms.DataAccess.Entities;
using Forms.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace Forms.Controllers
{
    public class HomeController : Controller
    {
        private readonly FormsDbContext _context;
        private readonly IUserService _userService;

        public HomeController(FormsDbContext context,IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var isCorrect = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId);

                if (isCorrect)
                {
                    var user = await _userService.GetUser(userId, _context);

                    var settings = user?.UserSetting;

                    if (settings is null)
                    {
                        settings = new UserSettingEntity();
                    }

                    ViewBag.Settings = settings;

                    var templates = await _context.Templates
                        .AsNoTracking()
                        .Where(t => t.IsPublic == 1 || t.TemplateAccess.Contains(user.Id) || t.CreatedById == user.Id)
                        .ToListAsync();

                    if(user.IsAdmin == 1)
                    {
                        templates = await _context.Templates
                            .AsNoTracking()
                            .ToListAsync();
                    }

                    return View(templates);
                }
            }
            else
            {
                var settings = new UserSettingEntity();

                ViewBag.Settings = settings;
            }

            return View(await _context.Templates
                .AsNoTracking()
                .Where(t => t.IsPublic == 1)
                .ToListAsync());
        }

        public async Task<IActionResult> GetTags()
        {
            var tags = await _context.Templates
                .AsNoTracking()
                .Select(t => t.Tags)
                .ToListAsync();

            var resultTags = new List<string>();

            foreach(var tagList in tags)
            {
                foreach(var tag in tagList)
                {
                    if (!resultTags.Contains(tag))
                    {
                        resultTags.Add(tag);
                    }
                }
            }
            return Json(resultTags);
        }
    }
}
