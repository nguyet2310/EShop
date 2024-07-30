using EShop.Models;
using EShop.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(DataContext dataContext ,UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dataContext=dataContext;
            _userManager=userManager;
            _roleManager=roleManager;
        }
        //public async Task<IActionResult> Index()
        //{
        //    var usersWithRoles = await (from u in _dataContext.Users
        //                                join ur in _dataContext.UserRoles on u.Id equals ur.UserId
        //                                join r in _dataContext.Roles on ur.RoleId equals r.Id
        //                                select new { User = u, RoleName = r.Name }).ToListAsync();
        //    return View(usersWithRoles);
        //    //return View(await _userManager.Users.OrderByDescending(x => x.Id).ToListAsync());
        //}
        public async Task<IActionResult> Index(int pg = 1)
        {
            var usersWithRoles = await (from u in _dataContext.Users
                                        join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                        join r in _dataContext.Roles on ur.RoleId equals r.Id
                                        select new { User = u, RoleName = r.Name }).ToListAsync(); //33 datas

            const int pageSize = 10; //10 items/trang

            if (pg < 1) //page < 1;
            {
                pg = 1; //page ==1
            }
            int recsCount = usersWithRoles.Count(); //33 items;

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize; //(3 - 1) * 10; 

            //category.Skip(20).Take(10).ToList()

            var data = usersWithRoles.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if (ModelState.IsValid)
            {
                var createdUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (createdUserResult.Succeeded)
                {
                    var createdUser = await _userManager.FindByEmailAsync(user.Email);
                    //var userId = createdUser.Id;
                    var role = _roleManager.FindByIdAsync(user.RoleId); // lấy roleId
                    //gán quyền
                    var addToRoleResult = await _userManager.AddToRoleAsync(createdUser, role.Result.Name);
                    if(!addToRoleResult.Succeeded)
                    {
                        AddIdentityErrors(addToRoleResult);
                    }
                    return RedirectToAction("Index", "User");
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

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AppUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //Update other user properties (excluding password)
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.RoleId = user.RoleId;

                var updatedUserResult = await _userManager.UpdateAsync(existingUser);
                if (updatedUserResult.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(updatedUserResult);
                    return View(existingUser);
                }
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            //Model validation failed
            TempData["error"] = "Model có một vài thứ đang bị lỗi";
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e=>e.ErrorMessage)).ToList();
            string errorMessage = string.Join("\n", errors);

            return View(existingUser);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var deletedResult = await _userManager.DeleteAsync(user);
            if (!deletedResult.Succeeded)
            {
                return View("Error");
            }
            TempData["Success"] = "Người dùng đã được xóa thành công!";
            return RedirectToAction("Index");
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
