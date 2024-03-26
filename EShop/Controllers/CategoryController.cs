using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
