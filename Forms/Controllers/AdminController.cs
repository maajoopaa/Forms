using Forms.DataAccess;
using Forms.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forms.Controllers
{
    public class AdminController : Controller
    {
        private readonly FormsDbContext _context;
        private readonly IUserService _userService;

        public AdminController(FormsDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [Authorize]
        public IActionResult Block(List<int> userIds)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return StatusCode(403);
            }
            try
            {
                userIds.ForEach(async u => await _userService.BlockUser(u,_context));

                return StatusCode(200);
            }
            catch
            {
                return StatusCode(404);
            }
        }

        [Authorize]
        public IActionResult Unblock(List<int> userIds)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return StatusCode(403);
            }

            try
            {
                userIds.ForEach(async u => await _userService.UnblockUser(u, _context));

                return StatusCode(200);
            }
            catch
            {
                return StatusCode(404);
            }
        }

        [Authorize]
        public IActionResult PromoteToAdmin(List<int> userIds)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return StatusCode(403);
            }
            try
            {
                userIds.ForEach(async u => await _userService.PromoteToAdmin(u, _context));
                return StatusCode(200);
            }
            catch
            {
                return StatusCode(404);
            }
        }

        [Authorize]
        public IActionResult LowerFromAdmin([FromBody]List<int> userIds)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return StatusCode(403);
            }
            try
            {
                userIds.ForEach(async u => await _userService.RemoveFromAdmin(u, _context));

                return StatusCode(200);
            }
            catch
            {
                return StatusCode(404);
            }
        }

        [Authorize]
        public IActionResult Remove(List<int> userIds)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return StatusCode(403);
            }
            try
            {
                userIds.ForEach(async u => await _userService.RemoveUser(u, _context));

                return StatusCode(200);
            }
            catch
            {
                return StatusCode(404);
            }
        }
    }
}
