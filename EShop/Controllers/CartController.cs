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

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
