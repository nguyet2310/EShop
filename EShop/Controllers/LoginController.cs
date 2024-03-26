using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
