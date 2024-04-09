using EShop.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Tên Sản phẩm")]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Mô tả Sản phẩm")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Giá Sản phẩm")]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price { get; set; }
        public string Image { get; set; } = "noimgage.jpg";
        [Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một thương hiệu")]
        public int BrandId { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một danh mục")]
        public int CategoryId { get; set; }
        public BrandModel Brand { get; set; }
        public CategoryModel Category { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload { get; set; }
    }
}
