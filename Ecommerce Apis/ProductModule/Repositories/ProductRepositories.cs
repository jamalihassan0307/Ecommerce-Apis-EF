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
            // Use the execution strategy provided by the context
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                // Start a transaction
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Add product to the database
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    // Process and add product images if provided
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

                    // Commit the transaction
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync();
                    return false;
                }
            });
        }

        public async Task<bool> UpdateProductAsync(Product product, UpdateProductRequestDTO request, IWebHostEnvironment environment)
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                // Begin a manual transaction within the execution strategy
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var existingProduct = await _context.Products.FindAsync(product.Id);
                    if (existingProduct == null) return false;

                    // Update product properties
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.StockQuantity = product.StockQuantity;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.ProductURL = product.ProductURL;
                    existingProduct.UpdatedAt = DateTime.UtcNow;

                    // Handle images to delete
                    if (request.ImageIdsToDelete?.Any() == true)
                    {
                        var imagesToDelete = await _context.ProductImages
                            .Where(pi => request.ImageIdsToDelete.Contains(pi.Id))
                            .ToListAsync();
                        _context.ProductImages.RemoveRange(imagesToDelete);
                    }

                    // Handle new images to add
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

                    // Save changes and commit the transaction
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw; // Re-throw the exception to propagate the error
                }
            });
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
            // Use the execution strategy provided by the context
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Fetch the product with associated images
                    var product = await _context.Products
                        .Include(p => p.ProductImages)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (product == null)
                    {
                        // No product found, rollback transaction
                        await transaction.RollbackAsync();
                        return false;
                    }

                    // Remove the product and its associated images
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    // Rollback transaction in case of an error
                    await transaction.RollbackAsync();
                    return false;
                }
            });
        }

    }
}
