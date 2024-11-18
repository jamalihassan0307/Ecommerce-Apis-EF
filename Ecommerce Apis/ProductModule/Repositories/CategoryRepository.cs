using Ecommerce_Apis.Data;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Apis.ProductModule.DTOs;
using Ecommerce_Apis.ProductModule.Models;
using Ecommerce_Apis.ProductModule.Repositories.InterFace;

namespace Ecommerce_Apis.ProductModule.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddCategory(CategoryRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
                ParentId = request.ParentId == 0 ? null : request.ParentId,
                ImagePath = request.ImagePath
            };

            _context.Categories.Add(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCategory(UpdateCategoryDTO request)
        {
            var category = await _context.Categories.FindAsync(request.Id);
            if (category == null) return false;

            category.Name = request.Name;
            category.ParentId = request.ParentId == 0 ? null : request.ParentId;
            category.ImagePath = request.ImagePath;

            _context.Categories.Update(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public List<CategoryDTO> GetAllCategories()
        {
            return _context.Categories
                .Include(c => c.ParentCategory)
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    ParentName = c.ParentCategory.Name,
                    ImagePath = c.ImagePath
                })
                .ToList();
        }

        public async Task<CategoryDTO?> GetCategoryById(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    ImagePath = c.ImagePath
                })
                .FirstOrDefaultAsync();
        }
    }
}
