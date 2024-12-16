
namespace Ecommerce_Apis.ProductModule.DTOs
{
    public class AddCategoryRequest
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public IFormFile ImagePath { get; set; }
        public IFormFile Icon { get; set; }
    }


        public class CategoryRequest
        {
            public string Name { get; set; }
            public int? ParentId { get; set; }
            public string ImagePath { get; set; }
            public string Icon { get; set; }


        }
}
