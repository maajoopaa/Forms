using Forms.DataAccess;
using Forms.DataAccess.Entities;
using Forms.Models.Account;
using Forms.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Security.Claims;

namespace Forms.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly FormsDbContext _context;
        private readonly IUserService _userService;

        public AccountController(FormsDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IActionResult> SignIn(string? returnUrl, string? username, string? password)
        {
            var settings = new UserSettingEntity();

            ViewBag.Settings = settings;

            if(username is not null && password is not null)
            {
                var userList = await _context.Users
                    .ToListAsync();

                var user = userList.FirstOrDefault(u => u.Username == username &&
                 _userService.VertifyPassword(password, u.PasswordHash));

                if (user is null)
                {
                    ViewBag.Error = "Неверный логин или пароль, перепроверьте данные!";

                    return PartialView(new AccountViewModel
                    {
                        loginViewModel = new LoginViewModel
                        {
                            Username = username,
                            Password = password
                        }
                    });
                }

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                };

                var claimIdentity = new ClaimsIdentity(claims, "Cookies");

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimIdentity));

                await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == user.Id)
                    .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.LastLogin, DateTime.UtcNow));

                await _context.SaveChangesAsync();

                return Redirect(returnUrl ?? "/Home/Index");
            }

            return PartialView(new AccountViewModel());
        }

        public async Task<IActionResult> Register(string? username, string? email, string? password)
        {
            var settings = new UserSettingEntity();

            ViewBag.Settings = settings;

            if (username is not null && email is not null && password is not null)
            {
                try
                {
                    var user = new UserEntity
                    {
                        Username = username,
                        Email = email,
                        PasswordHash = _userService.HashPassword(password),
                        CreatedAt = DateTime.UtcNow,
                        UserSetting = settings
                    };

                    await _context.Users
                        .AddAsync(user);

                    await _context.SaveChangesAsync();

                    return Redirect($"/Account/SignIn?username={username}&password={password}");
                }
                catch
                {
                    ViewBag.Error = "Пользователь с таким Логином или Почтой уже существует!";

                    return PartialView(new AccountViewModel
                    {
                        registerViewModel = new RegisterViewModel
                        {
                            Username = username,
                            Email = email,
                            Password = password
                        }
                    });
                }
            }

            return PartialView(new AccountViewModel());
        }
        [Authorize]
        public async Task<IActionResult> Quit()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/Home/Index");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var isCorrect = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId);

            if (isCorrect)
            {
                var user = await _userService.GetUser(userId, _context);

                var settings = new UserSettingEntity();

                if(user.UserSetting is not null)
                {
                    settings = user.UserSetting;
                }

                ViewBag.Context = _context;

                ViewBag.Settings = settings;

                return View(user);
            }

            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        public async Task<IActionResult> GetTemplates()
        {
            var isCorrect = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId);

            if (isCorrect)
            {
                var user = await _userService.GetUser(userId, _context);

                var settings = new UserSettingEntity();

                if (user.UserSetting is not null)
                {
                    settings = user.UserSetting;
                }

                ViewBag.Settings = settings;

                return PartialView("Views/Template/ViewList.cshtml", user.IsAdmin == 1 ? _context.Templates.OrderByDescending(t => t.CreatedAt).ToList() :
                    user.Templates.OrderByDescending(t => t.CreatedAt).ToList());
            }

            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        public async Task<IActionResult> GetForms()
        {
            var isCorrect = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId);

            if (isCorrect)
            {
                var user = await _userService.GetUser(userId, _context);

                var settings = new UserSettingEntity();

                if (user.UserSetting is not null)
                {
                    settings = user.UserSetting;
                }

                ViewBag.Settings = settings;

                return PartialView("Views/Form/ViewList.cshtml", user.IsAdmin == 1 ? _context.Forms.OrderByDescending(f => f.FilledAt).ToList() :
                    user.Forms.OrderByDescending(f => f.FilledAt).ToList());
            }

            else
            {
                return Redirect("/Home/Index");
            }

        }

        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var isCorrect = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId);

            if (isCorrect)
            {
                var user = await _userService.GetUser(userId, _context);

                var settings = new UserSettingEntity();

                if (user.UserSetting is not null)
                {
                    settings = user.UserSetting;
                }

                ViewBag.Settings = settings;

                return PartialView("Views/Admin/UserManagment.cshtml", await _context.Users
                    .AsNoTracking()
                    .OrderByDescending(u => u.LastLogin)
                    .ToListAsync());
            }

            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        public async Task<IActionResult> SaveSettings(string theme, string language)
        {
            var isCorrect = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId);

            if (isCorrect)
            {
                var user = await _userService.GetUser(userId, _context);

                try
                {
                    await _context.UserSettings
                        .AsNoTracking()
                        .Where(us => us.UserId == user.Id)
                        .ExecuteUpdateAsync(s => s
                        .SetProperty(us => us.Theme, theme)
                        .SetProperty(us => us.Language, language));

                    return Json(new { isValid = true });
                }
                catch
                {
                    return Json(new { isValid = false });
                }
            }

            else
            {
                return Redirect("/Home/Index");
            }
        }
    }
} 
