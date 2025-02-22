using Forms.DataAccess;
using Forms.DataAccess.Entities;
using Forms.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;

namespace Forms.Controllers
{
    public class FormController : Controller
    {
        private readonly FormsDbContext _context;

        private readonly IFormService _formService;
        private readonly IUserService _userService;

        public FormController(FormsDbContext context, IFormService formService, IUserService userService)
        {
            _context = context;
            _formService = formService;
            _userService = userService;
        }

        [Authorize]
        public async Task<IActionResult> Fill(int templateId, string answers)
        {
            if (answers is not null)
            {
                var filledById = (await GetUserFromCookies()).Id;

                var form = await _formService.FillForm(templateId, filledById, answers, _context);

                if (form is null)
                {
                    ViewBag.Error = "Во время сохранения произошла ошибка. Пожалуйста, перепроверьте данные!";

                    return RedirectToAction("View","Template",form);
                }

                return RedirectToAction("Profile","Account");
            }
            return RedirectToAction("View", "Template");
        }

        [Authorize]
        public async Task<IActionResult> View(int formId)
        {
            var form = await _formService.GetFormById(formId, _context);

            var themes = await _context.Themes
                    .ToListAsync();

            var user = await GetUserFromCookies();

            ViewBag.Themes = themes;

            ViewBag.Context = _context;

            ViewBag.Settings = user.UserSetting;

            return form.FilledById == user.Id || user.IsAdmin == 1 ? Redirect($"/Form/Edit?formId={formId}") : PartialView(form);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int formId, string answers)
        {
            var themes = await _context.Themes
                    .ToListAsync();

            var user = await GetUserFromCookies();

            ViewBag.Themes = themes;

            ViewBag.Context = _context;

            ViewBag.Settings = user.UserSetting;

            if (answers is not null)
            {
                var form = await _formService.UpdateForm(formId, answers, _context);

                if (form is null)
                {
                    ViewBag.Error = "Во время сохранения произошла ошибка. Пожалуйста, перепроверьте данные!";

                    return View(form);
                }

                return RedirectToAction("Profile","Account");
            }

            var currentForm = await _formService.GetFormById(formId, _context);

            return PartialView(currentForm);
        }

        [Authorize]
        public async Task<IActionResult> Remove(int formId)
        {
            var isCorrect = await _formService.RemoveForm(formId, _context);

            if (!isCorrect)
            {
                ViewBag.Error = "Во время удаления произошла ошибка. Попробуйте еще раз.";

                return Redirect($"/Form/Edit?formId={formId}");
            }

            return RedirectToAction("Profile", "Account");
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
