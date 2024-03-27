using EShop.Models;
using EShop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;

        public CategoryController(DataContext context) 
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string slug="")
        {
            CategoryModel category = _dataContext.Categories.Where(c => c.Slug == slug).FirstOrDefault();
            if(category == null)
            {
                return RedirectToAction("Index");
            }

            var productsByCategory = _dataContext.Products.Where(p => p.CategoryId == category.Id);

            return View(await productsByCategory.OrderByDescending(p => p.Id).ToListAsync());
        }

    }
}
