using EShop.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly DataContext _dataContext;

        public RoleController(DataContext dataContext)
        {
            _dataContext=dataContext;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Roles.OrderByDescending(r => r.Id).ToListAsync());
        }
    }
}
