using EShop.Models;
using EShop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    public class AccountController : Controller
    {
		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AccountController(UserManager<AppUserModel> userManager, 
			SignInManager<AppUserModel> signInManager, RoleManager<IdentityRole> roleManager)
        {
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}
		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Create()
        {
            return View();
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AppUserModel user)
		{
			if(ModelState.IsValid)
			{
				//AppUserModel newUser = new AppUserModel { UserName=user.UserName, Email=user.Email };
				var createdUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
				if (createdUserResult.Succeeded)
				{
					var createdUser = await _userManager.FindByEmailAsync(user.Email);
					//var userId = createdUser.Id;
					//var role = _roleManager.FindByIdAsync(user.RoleId); // lấy roleId
																		//gán quyền
					var addToRoleResult = await _userManager.AddToRoleAsync(createdUser, "User");
					if (!addToRoleResult.Succeeded)
					{
						AddIdentityErrors(addToRoleResult);
					}
					TempData["success"] = "Tạo user thành công";
					return Redirect("/Account/Login");
				}
				else
				{
					AddIdentityErrors(createdUserResult);
					return View(user);
				}
			}
			else
			{
				TempData["error"] = "Model có một vài thứ đang bị lỗi";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMas = string.Join("\n", errors);
				return BadRequest(errorMas);
			}
		}

		public IActionResult Login(string returnUrl)
		{
			return View(new LoginViewModel { ReturnUrl= returnUrl });
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if(ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
				if (result.Succeeded)
				{
					return Redirect(loginVM.ReturnUrl ?? "/");
				}
				ModelState.AddModelError("", "username hoặc password bị sai!");
			}
			return View(loginVM);
		}

		public async Task<IActionResult> Logout(string returnUrl = "/")
		{
			await _signInManager.SignOutAsync();
			return Redirect(returnUrl);
		}

		private void AddIdentityErrors(IdentityResult identityResult)
		{
			foreach (var error in identityResult.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}
	}
}
