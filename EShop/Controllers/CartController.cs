using EShop.Models;
using EShop.Models.ViewModels;
using EShop.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;

        public CartController(DataContext dataContext)
        {
            _dataContext=dataContext;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new ()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };
            return View(cartVM);
        }
        public IActionResult Checkout()
        {
            return View();
        }
    }
}
