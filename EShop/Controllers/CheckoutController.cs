using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
