using Microsoft.EntityFrameworkCore;

using Ecommerce_Apis.CouponModule.Models;
using Ecommerce_Apis.CouponModule.DTOs;
using Ecommerce_Apis.CouponModule.Repositories.InterFace;
using Ecommerce_Apis.Data;
using Ecommerce_Apis.OrderModule.DTOs;

namespace Ecommerce_Apis.CouponModule.Repositories
{
    public class CouponRepositories : ICouponRepositories
    {
        private readonly ApplicationDbContext _context;

        public CouponRepositories(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateCoupon(CouponDTO couponDTO)
        {
            var coupon = new Coupon
            {
                Code = couponDTO.Code,
                Discount = couponDTO.Discount,
                DiscountType = couponDTO.DiscountType,
                ExpirationDate = couponDTO.ExpirationDate
            };

            _context.Coupons.Add(coupon);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<CouponDTO>> GetAllCoupons()
        {
            var currentDate = DateTime.UtcNow;
            return await _context.Coupons
                .Where(c => c.ExpirationDate > currentDate)
                .Select(c => new CouponDTO
                {
                    Id = c.Id,
                    Code = c.Code,
                    Discount = c.Discount,
                    DiscountType = c.DiscountType,
                    ExpirationDate = c.ExpirationDate
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateCoupon(CouponDTO couponDTO)
        {
            var coupon = await _context.Coupons.FindAsync(couponDTO.Id);
            if (coupon == null) return false;

            coupon.Code = couponDTO.Code;
            coupon.Discount = couponDTO.Discount;
            coupon.DiscountType = couponDTO.DiscountType;
            coupon.ExpirationDate = couponDTO.ExpirationDate;

            _context.Coupons.Update(coupon);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return false;

            _context.Coupons.Remove(coupon);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ApplyCoupon(ApplyCouponRequestDTO request)
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && c.ExpirationDate > DateTime.UtcNow);

            if (coupon == null) return false;

            if (request.ProductId.HasValue)
            {
                var couponProduct = new CouponProduct
                {
                    CouponId = coupon.Id,
                    ProductId = request.ProductId.Value
                };
                _context.CouponProducts.Add(couponProduct);
            }
            else if (request.CategoryId.HasValue)
            {
                var couponCategory = new CouponCategory
                {
                    CouponId = coupon.Id,
                    CategoryId = request.CategoryId.Value
                };
                _context.CouponCategories.Add(couponCategory);
            }
            else
            {
                return false;
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<RandomCouponProductDTO>> GetRandomCouponProductsAsync()
        {
            var currentDate = DateTime.UtcNow;
            return await _context.CouponProducts
                .Include(cp => cp.Product)
                .ThenInclude(p => p.ProductImages)
                .Include(cp => cp.Coupon)
                .Where(cp => cp.Coupon.ExpirationDate > currentDate)
                .Select(cp => new RandomCouponProductDTO
                {
                    ProductId = cp.Product.Id,
                    ProductName = cp.Product.Name,
                    Description = cp.Product.Description,
                    Price = cp.Product.Price,
                    ProductURL = cp.Product.ProductURL,
                    StockQuantity = cp.Product.StockQuantity,
                    CouponId = cp.Coupon.Id.ToString(),
                    Discount = cp.Coupon.Discount,
                    DiscountType = cp.Coupon.DiscountType,
                    ExpirationDate = cp.Coupon.ExpirationDate,
                    DiscountedPrice = cp.Coupon.DiscountType == "PERCENTAGE" 
                        ? cp.Product.Price * (1 - cp.Coupon.Discount / 100)
                        : Math.Max(cp.Product.Price - cp.Coupon.Discount, cp.Product.Price * 0.7m),
                    ImagePaths = cp.Product.ProductImages.Select(pi => pi.ImagePath).ToList()
                })
                .OrderBy(r => Guid.NewGuid())
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<RandomCouponProductDTO>> GetProductsByCouponAsync(string couponId)
        {
            return await _context.CouponProducts
                .Include(cp => cp.Product)
                .ThenInclude(p => p.ProductImages)
                .Include(cp => cp.Coupon)
                .Where(cp => cp.Coupon.Id.ToString() == couponId && cp.Coupon.ExpirationDate > DateTime.UtcNow)
                .Select(cp => new RandomCouponProductDTO
                {
                    ProductId = cp.Product.Id,
                    ProductName = cp.Product.Name,
                    Description = cp.Product.Description,
                    Price = cp.Product.Price,
                    ProductURL = cp.Product.ProductURL,
                    StockQuantity = cp.Product.StockQuantity,
                    CouponId = cp.Coupon.Id.ToString(),
                    Discount = cp.Coupon.Discount,
                    DiscountType = cp.Coupon.DiscountType,
                    ExpirationDate = cp.Coupon.ExpirationDate,
                    DiscountedPrice = CalculateDiscountedPrice(cp.Product.Price, cp.Coupon),
                    ImagePaths = cp.Product.ProductImages.Select(pi => pi.ImagePath).ToList()
                })
                .Take(4)
                .ToListAsync();
        }

        public async Task<List<CouponWithProductsDTO>> GetCouponsWithProductsAsync()
        {
            var currentDate = DateTime.UtcNow;
            var coupons = await _context.Coupons
                .Where(c => c.ExpirationDate > currentDate)
                .Include(c => c.CouponProducts)
                .ThenInclude(cp => cp.Product)
                .ThenInclude(p => p.ProductImages)
                .Select(c => new CouponWithProductsDTO
                {
                    Coupon = new CouponDTO
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Discount = c.Discount,
                        DiscountType = c.DiscountType,
                        ExpirationDate = c.ExpirationDate
                    },
                    Products = c.CouponProducts.Select(cp => new RandomCouponProductDTO
                    {
                        ProductId = cp.Product.Id,
                        ProductName = cp.Product.Name,
                        Description = cp.Product.Description,
                        Price = cp.Product.Price,
                        ProductURL = cp.Product.ProductURL,
                        StockQuantity = cp.Product.StockQuantity,
                        CouponId = c.Id.ToString(),
                        Discount = c.Discount,
                        DiscountType = c.DiscountType,
                        ExpirationDate = c.ExpirationDate,
                        DiscountedPrice = CalculateDiscountedPrice(cp.Product.Price, c),
                        ImagePaths = cp.Product.ProductImages.Select(pi => pi.ImagePath).ToList()
                    }).Take(4).ToList()
                })
                .ToListAsync();

            return coupons.Where(c => c.Products.Any()).ToList();
        }

        private static decimal CalculateDiscountedPrice(decimal originalPrice, Coupon coupon)
        {
            return coupon.DiscountType == "PERCENTAGE"
                ? originalPrice * (1 - coupon.Discount / 100)
                : Math.Max(originalPrice - coupon.Discount, originalPrice * 0.7m);
        }

        public async Task<RandomCouponProductDTO> GetProductByProductIdCouponIdAsync(string couponId, string productId)
        {
            return await _context.CouponProducts
                .Include(cp => cp.Product)
                .ThenInclude(p => p.ProductImages)
                .Include(cp => cp.Coupon)
                .Where(cp => cp.Coupon.Id.ToString() == couponId && cp.Product.Id.ToString() == productId)
                .Select(cp => new RandomCouponProductDTO
                {
                    ProductId = cp.Product.Id,
                    ProductName = cp.Product.Name,
                    Description = cp.Product.Description,
                    Price = cp.Product.Price,
                    ProductURL = cp.Product.ProductURL,
                    StockQuantity = cp.Product.StockQuantity,
                    CouponId = cp.Coupon.Id.ToString(),
                    Discount = cp.Coupon.Discount,
                    DiscountType = cp.Coupon.DiscountType,
                    ExpirationDate = cp.Coupon.ExpirationDate,
                    DiscountedPrice = CalculateDiscountedPrice(cp.Product.Price, cp.Coupon),
                    ImagePaths = cp.Product.ProductImages.Select(pi => pi.ImagePath).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CouponDTO> GetCouponById(int id)
        {
            return await _context.Coupons
                .Where(c => c.Id == id)
                .Select(c => new CouponDTO
                {
                    Id = c.Id,
                    Code = c.Code,
                    Discount = c.Discount,
                    DiscountType = c.DiscountType,
                    ExpirationDate = c.ExpirationDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<RandomCouponProductDTO>> GetProductsByDiscountPercentage(int discountPercentage)
        {
            return await _context.CouponProducts
                .Include(cp => cp.Product)
                .ThenInclude(p => p.ProductImages)
                .Include(cp => cp.Coupon)
                .Where(cp => cp.Coupon.DiscountType == "PERCENTAGE" && cp.Coupon.Discount == discountPercentage)
                .Select(cp => new RandomCouponProductDTO
                {
                    ProductId = cp.Product.Id,
                    ProductName = cp.Product.Name,
                    Description = cp.Product.Description,
                    Price = cp.Product.Price,
                    ProductURL = cp.Product.ProductURL,
                    StockQuantity = cp.Product.StockQuantity,
                    CouponId = cp.Coupon.Id.ToString(),
                    Discount = cp.Coupon.Discount,
                    DiscountType = cp.Coupon.DiscountType,
                    ExpirationDate = cp.Coupon.ExpirationDate,
                    DiscountedPrice = CalculateDiscountedPrice(cp.Product.Price, cp.Coupon),
                    ImagePaths = cp.Product.ProductImages.Select(pi => pi.ImagePath).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<RandomCouponProductDTO>> GetProductsByDiscountRange(decimal minDiscount, decimal maxDiscount)
        {
            return await _context.CouponProducts
                .Include(cp => cp.Product)
                .ThenInclude(p => p.ProductImages)
                .Include(cp => cp.Coupon)
                .Where(cp => cp.Coupon.DiscountType == "FLAT" && 
                             cp.Coupon.Discount >= minDiscount && 
                             cp.Coupon.Discount <= maxDiscount)
                .Select(cp => new RandomCouponProductDTO
                {
                    ProductId = cp.Product.Id,
                    ProductName = cp.Product.Name,
                    Description = cp.Product.Description,
                    Price = cp.Product.Price,
                    ProductURL = cp.Product.ProductURL,
                    StockQuantity = cp.Product.StockQuantity,
                    CouponId = cp.Coupon.Id.ToString(),
                    Discount = cp.Coupon.Discount,
                    DiscountType = cp.Coupon.DiscountType,
                    ExpirationDate = cp.Coupon.ExpirationDate,
                    DiscountedPrice = CalculateDiscountedPrice(cp.Product.Price, cp.Coupon),
                    ImagePaths = cp.Product.ProductImages.Select(pi => pi.ImagePath).ToList()
                })
                .ToListAsync();
        }

        // Implement remaining interface methods...
    }
}
