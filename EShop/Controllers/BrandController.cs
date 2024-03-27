using EShop.Models;
using EShop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string slug = "")
        {
            BrandModel brand = _dataContext.Brands.Where(c => c.Slug == slug).FirstOrDefault();
            if (brand == null)
            {
                return RedirectToAction("Index");
            }

            var productsByBrand = _dataContext.Products.Where(p => p.BrandId == brand.Id);

            return View(await productsByBrand.OrderByDescending(p => p.Id).ToListAsync());
        }
    }
}
