using Forms.DataAccess;
using Forms.DataAccess.Entities;
using Forms.Models.Template;
using Forms.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Forms.Controllers
{
    public class TemplateController : Controller
    {
        private readonly FormsDbContext _context;
        private readonly ITemplateService _templateService;
        private readonly IUserService _userService;

        public TemplateController(FormsDbContext context, ITemplateService templateService, IUserService userService)
        {
            _context = context;
            _templateService = templateService;
            _userService = userService;
        }

        [Authorize]
        public async Task<IActionResult> Create(string title, string description, int theme, string? imageUrl, string tags, byte isPublic,
            string questions, string? templateAccess)
        {
            var user = await GetUserFromCookies();

            var themes = await _context.Themes
                .ToListAsync();

            ViewBag.Settings = user.UserSetting;

            ViewBag.Themes = themes;

            if (!(title is null || description is null || tags is null)) 
            {

                var template = await _templateService.CreateTemplate(title, description,theme,imageUrl,tags, isPublic,
                    user.Id, questions, templateAccess,_context);

                if(template is null)
                {
                    ViewBag.Error = "Данные введены неверно, проверьте еще раз!";

                    return PartialView(template);
                }

                return RedirectToAction("Profile","Account");
            }

            return PartialView();
        }

        [Authorize]
        public async Task<IActionResult> Edit(int templateId, string title, string description, int theme, string? imageUrl, string tags, byte isPublic, string questions,
            string templateAccess)
        {
            var user = await GetUserFromCookies();

            var themes = await _context.Themes
                .ToListAsync();

            ViewBag.Settings = user.UserSetting;

            ViewBag.Themes = themes;

            ViewBag.Context = _context;

            ViewBag.User = user;

            if (!(title is null))
            {
                var template = await _templateService.UpdateTemplate(templateId,title,description,theme,imageUrl,tags,isPublic,questions,templateAccess,_context);

                if(template is null)
                {
                    ViewBag.Error = "Данные введены неверно, проверьте еще раз!";

                    return PartialView(template);
                }

                return RedirectToAction("Profile","Account");
            }

            var currentTemplate = await _templateService.GetTemplateById(templateId,_context);

            return PartialView(currentTemplate);
        }

        [Authorize]
        public async Task<IActionResult> Remove(int templateId)
        {
            var isCorrect = await _templateService.RemoveTemplate(templateId,_context);

            if (!isCorrect)
            {
                ViewBag.Error = "Во время удаления произошла ошибка. Попробуйте еще раз.";

                return Redirect($"/Template/Update?templateId={templateId}");
            }

            return Redirect("/Account/Profile");
        }

        public async Task<IActionResult> SearchTemplate(string searchQuery)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserFromCookies();

                ViewBag.Settings = user.UserSetting;
            }
            else
            {
                ViewBag.Settings = new UserSettingEntity();
            }

            var templates = await _templateService.SearchTemplates(searchQuery,_context);

            return View("~/Views/Template/ViewList.cshtml",templates);
        }

        public async Task<IActionResult> SearchByTag(string tag)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserFromCookies();

                ViewBag.Settings = user.UserSetting;
            }
            else
            {
                ViewBag.Settings = new UserSettingEntity();
            }

            var templates = await _context.Templates
                .AsNoTracking()
                .Where(t => t.Tags.Contains(tag))
                .ToListAsync();

            return View("~/Views/Template/ViewList.cshtml", templates);
        }

        public async Task<IActionResult> View(int templateId)
        {
            var template = await _templateService.GetTemplateById(templateId,_context);

            var themes = await _context.Themes
                    .ToListAsync();

            ViewBag.Themes = themes;

            ViewBag.Context = _context;

            ViewBag.User = null;

            if (User.Identity.IsAuthenticated)
            {
                var user = await GetUserFromCookies();

                ViewBag.User = user;

                ViewBag.Settings = user.UserSetting;

                if(template.IsPublic == 1 || template.TemplateAccess.Contains(user.Id) || template.CreatedById == user.Id || user.IsAdmin == 1)
                {
                    return template.CreatedById == user.Id || user.IsAdmin == 1 ? Redirect($"/Template/Edit?templateId={templateId}") : PartialView(template);
                }
            }

            ViewBag.Settings = new UserSettingEntity();
            if(template.IsPublic == 1)
            {
                return PartialView("ViewReadOnly", template);
            }

            return RedirectToAction("Index","Home");
        }

        [Authorize]
        public async Task<IActionResult> GetUsers(string property, string query)
        {
            var users = new List<UserEntity>();

            var resultUsers = new List<UserModel>();

            switch (property)
            {
                case "email":
                    users = await _context.Users
                        .AsNoTracking()
                        .Where(u => u.Email.ToLower().StartsWith(query.ToLower()))
                        .ToListAsync();

                    users.ForEach(u => resultUsers.Add(new UserModel
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                    }));

                    break;

                case "username":
                    users = await _context.Users
                        .AsNoTracking()
                        .Where(u => u.Username.ToLower().StartsWith(query.ToLower()))
                        .ToListAsync();

                    users.ForEach(u => resultUsers.Add(new UserModel
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                    }));

                    break;

                default:
                    return Json(new List<UserModel>());
            }

            return Json(resultUsers);
        }

        [Authorize]
        public async Task<IActionResult> GetTags(string query)
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
                    if (tag.ToLower().StartsWith(query.ToLower()) && !resultTags.Contains(tag))
                    {
                        resultTags.Add(tag);
                    }
                }
            }

            return Json(resultTags);
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
