
using AutoMapper;
using Ecommerce_Apis.ProductModule.DTOs;
using Ecommerce_Apis.ProductModule.Models;

namespace Ecommerce_Apis.ProductModule.Repositories
{
    public interface IProductRepository
    {
        Task<bool> CreateProductAsync(Product addProductRequest, AddUserProductRequest request, IWebHostEnvironment environment);
        Task<bool> UpdateProductAsync(Product product, UpdateProductRequestDTO request, IWebHostEnvironment environment);
        Task<List<ProductDTO>?> FilterProductsCategory(int parentId, IMapper mapper);
        Task<List<ProductDTO>?> SearchProductsByName(string query, IMapper mapper);
        Task<List<ProductDTO>?> GetProductsWithPaging(int pageNumber, int pageSize, IMapper mapper);
        Task<ProductDTOWithImageId> GetProductById(int productId, IMapper mapper);
        Task<ProductDTO> GetProductByURL(string url, IMapper mapper);
        Task<bool> UpdateProductStock(UpdateProductStockDTO request);
        Task<bool> DeleteProduct(int id);
    }
}
