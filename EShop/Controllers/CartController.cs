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
            // Lấy giỏ hàng từ session, nếu không có sẽ tạo mới
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

        public async Task<IActionResult> Add(int id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(id);

            // Lấy giỏ hàng từ session với từ khóa "Cart" và chuyển đổi dữ liệu đó thành
            // một đối tượng kiểu List<CartItemModel>, nếu không có sẽ tạo mới
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tiến hành thêm sản phẩm vào giỏ hàng
            CartItemModel cartItems = cart.Where(c => c.ProductId==id).FirstOrDefault();

            if(cartItems==null)
            {
                cart.Add(new CartItemModel(product));
            }
            else
            {
                cartItems.Quantity += 1;
            }

            // Lưu lại danh sách giỏ hàng đã cập nhật vào session
            HttpContext.Session.SetJson("Cart", cart);

            TempData["success"] = "Add Item to cart successfully";

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Decrease(int id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");

            CartItemModel cartItem= cart.Where(c=> c.ProductId==id).FirstOrDefault();
            if(cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(c => c.ProductId==id);
            }
            if(cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["success"] = "Decrease Item quantity to cart successfully";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(int id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");

            CartItemModel cartItem = cart.Where(c => c.ProductId==id).FirstOrDefault();
            if (cartItem.Quantity >= 1)
            {
                ++cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(c => c.ProductId==id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["success"] = "Increase Item quantity to cart successfully";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");

            cart.RemoveAll(p => p.ProductId==id);
            if(cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["success"] = "Remove Item of cart successfully";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Clear all Item of cart successfully";
            return RedirectToAction("Index");
        }
    }
}
