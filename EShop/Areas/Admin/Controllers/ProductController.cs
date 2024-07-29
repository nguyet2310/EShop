using EShop.Models;
using EShop.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Publisher, Author")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext=dataContext;
            _webHostEnvironment=webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _dataContext.Products.OrderByDescending(p => p.Id).Include(p => p.Category).Include(p => p.Brand).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;
                }

                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm sản phẩm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                //List<string> errors = new List<string>();
                //foreach (var value in ModelState.Values)
                //{
                //    foreach (var error in value.Errors)
                //    {
                //        errors.Add(error.ErrorMessage);
                //    }
                //}
                //string errorMas = string.Join("\n", errors);
                //return BadRequest(errorMas);
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            //var existed_product = _dataContext.Products.Find(product.Id);//tim sp theo id product

            if (ModelState.IsValid)
            {
                // Lấy sản phẩm từ cơ sở dữ liệu
                var existingProduct = await _dataContext.Products.FindAsync(product.Id);

                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null && slug.Id != product.Id)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(product);
                }
                else
                {
                    if (product.ImageUpload != null)
                    {
                        string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                        string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                        string filePath = Path.Combine(uploadsDir, imageName);

                        //delete old picture
                        string oldfileImage = Path.Combine(uploadsDir, existingProduct.Image);
                        try
                        {
                            if (System.IO.File.Exists(oldfileImage))
                            {
                                System.IO.File.Delete(oldfileImage);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "An error occurred while deleting the product image.");
                        }

                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        await product.ImageUpload.CopyToAsync(fs);
                        fs.Close();
                        product.Image = imageName;

                        //existed_product.Image = imageName;
                    }

                    // Update other product properties
                    //existed_product.Name = product.Name;
                    //existed_product.Description = product.Description;
                    //existed_product.Price = product.Price;
                    //existed_product.CategoryId = product.CategoryId;
                    //existed_product.BrandId = product.BrandId;
                    //...other properties

                    //_dataContext.Update(existed_product); //Update the existing product object

                    else
                    {
                        // Lưu trữ giá trị hiện tại của product.Image vào một biến tạm thời
                        //string currentImage = product.Image;

                        // Gán lại product.Image bằng giá trị của biến tạm thời
                        product.Image = existingProduct.Image;
                    }

                    //_dataContext.Update(product);
                    // Cập nhật trạng thái của existingProduct với dữ liệu mới từ product
                    _dataContext.Entry(existingProduct).CurrentValues.SetValues(product);

                    await _dataContext.SaveChangesAsync();
                    TempData["success"] = "Cập nhật sản phẩm thành công";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                //List<string> errors = new List<string>();
                //foreach (var value in ModelState.Values)
                //{
                //    foreach (var error in value.Errors)
                //    {
                //        errors.Add(error.ErrorMessage);
                //    }
                //}
                //string errorMas = string.Join("\n", errors);
                //return BadRequest(errorMas);
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(); //Handle product not found
            }
            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
            string oldfileImage = Path.Combine(uploadsDir, product.Image);
            try
            {
                if(System.IO.File.Exists(oldfileImage))
                {
                    System.IO.File.Delete(oldfileImage);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the product image.");
            }
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Sản phẩm đã xóa"
;           return RedirectToAction("Index");
        }
    }
}
