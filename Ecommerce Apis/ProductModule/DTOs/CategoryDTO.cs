namespace Ecommerce_Apis.ProductModule.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public string ImagePath { get; set; }
        public string Icon { get; set; }
    }

    public class UpdateCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string ImagePath { get; set; }
        public string Icon { get; set; }
    }
    public class UpdateCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public IFormFile ImagePath { get; set; }
        public IFormFile Icon { get; set; }
    }
} 