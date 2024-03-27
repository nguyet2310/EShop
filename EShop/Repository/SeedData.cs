using EShop.Models;
using Microsoft.EntityFrameworkCore;

namespace EShop.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();
            if(!_context.Products.Any() ) //kiem tra du lieu chua co trong product
            {
                CategoryModel macbook = new CategoryModel { Name="Macbook", Slug="macbook", Description="Macbook is large product in the world", Status=1 };
                CategoryModel pc = new CategoryModel { Name="Pc", Slug="pc", Description="Pc is large brand in the world", Status=1 };

                BrandModel apple = new BrandModel { Name="Apple", Slug="apple", Description="Apple is large brand in the world", Status=1};
                BrandModel samsung = new BrandModel { Name="Samsung", Slug="samsung", Description="Samsung is large brand in the world", Status=1};

                _context.Products.AddRange(
                    new ProductModel { Name="Macbook", Slug="macbook", Description="Macbook is best", Image="1.jpg", Category = macbook, Brand = apple, Price=1233 },
                    new ProductModel { Name="Pc", Slug="pc", Description="Pc is best", Image="2.jpg", Category = pc, Brand = samsung, Price=1233 }
                );
                _context.SaveChanges();
            }
        }
    }
}
