using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RopeyDVDs.Models.ViewModels;
using RopeyDVDs.Service;

namespace RopeyDVDs.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(IUserService userService, UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
             return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var AuthUser = _userService.GetUser();
            var User = await _userManager.FindByNameAsync(AuthUser);
            var result = await _userManager.ChangePasswordAsync(User, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {

            }
            else
            {

            }
            
            return View();
        }

    }
}
