using Microsoft.EntityFrameworkCore;
using Ecommerce_Apis.ProductModule.DTOs;
using Ecommerce_Apis.ProductModule.Models;
using AutoMapper;
using Ecommerce_Apis.Utills;
using Ecommerce_Apis.Data;

namespace Ecommerce_Apis.ProductModule.Repositories
{
    public class ProductRepositories : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepositories(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateProductAsync(Product product, AddUserProductRequest request, IWebHostEnvironment environment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                if (request.Images != null && request.Images.Count > 0)
                {
                    foreach (var image in request.Images)
                    {
                        string imagePath = await FileManage.UploadAsync(image, environment);
                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImagePath = imagePath,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.ProductImages.Add(productImage);
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(Product product, UpdateProductRequestDTO request, IWebHostEnvironment environment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null) return false;

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ProductURL = product.ProductURL;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                if (request.ImageIdsToDelete?.Any() == true)
                {
                    var imagesToDelete = await _context.ProductImages
                        .Where(pi => request.ImageIdsToDelete.Contains(pi.Id))
                        .ToListAsync();
                    _context.ProductImages.RemoveRange(imagesToDelete);
                }

                if (request.Images?.Any() == true)
                {
                    foreach (var image in request.Images)
                    {
                        string imagePath = await FileManage.UploadAsync(image, environment);
                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImagePath = imagePath,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.ProductImages.Add(productImage);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<List<ProductDTO>> FilterProductsCategory(int parentId, IMapper mapper)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.CategoryId == parentId)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ProductURL = p.ProductURL,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ImagePath = p.ProductImages.Select(pi => pi.ImagePath).ToList(),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<List<ProductDTO>> SearchProductsByName(string query, IMapper mapper)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.Name.Contains(query))
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ProductURL = p.ProductURL,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ImagePath = p.ProductImages.Select(pi => pi.ImagePath).ToList(),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<List<ProductDTO>> GetProductsWithPaging(int pageNumber, int pageSize, IMapper mapper)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ProductURL = p.ProductURL,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ImagePath = p.ProductImages.Select(pi => pi.ImagePath).ToList(),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<ProductDTOWithImageId> GetProductById(int productId, IMapper mapper)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.Id == productId)
                .Select(p => new ProductDTOWithImageId
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ProductURL = p.ProductURL,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ImagePath = p.ProductImages.Select(pi => new ProductImageWithId 
                    { 
                        Id = pi.Id, 
                        ImagePath = pi.ImagePath 
                    }).ToList(),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ProductDTO> GetProductByURL(string url, IMapper mapper)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.ProductURL == url)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ProductURL = p.ProductURL,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ImagePath = p.ProductImages.Select(pi => pi.ImagePath).ToList(),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateProductStock(UpdateProductStockDTO request)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null) return false;

            product.StockQuantity = request.NewStockQuantity;
            return await _context.SaveChangesAsync() > 0;
        }

       

        public async Task<bool> DeleteProduct(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var product = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null) return false;

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

     }
}
