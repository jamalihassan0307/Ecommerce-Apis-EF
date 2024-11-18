using Ecommerce_Apis.ProductModule.Models;
using Ecommerce_Apis.ProductModule.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce_Apis.ProductModule.Repositories.InterFace
{
    public interface ICategoryRepository
    {
        Task<bool> AddCategory(CategoryRequest request);
        Task<bool> DeleteCategory(int id);
        Task<bool> UpdateCategory(UpdateCategoryDTO request);
        List<CategoryDTO> GetAllCategories();
        Task<CategoryDTO?> GetCategoryById(int id); 
    }
}
