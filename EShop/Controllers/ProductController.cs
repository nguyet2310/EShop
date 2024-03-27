using EShop.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;

        public ProductController(DataContext dataContext)
        {
            _dataContext=dataContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            var productsById = _dataContext.Products.Where(p => p.Id == id).FirstOrDefault();

            return View(productsById);
        }
    }
}
