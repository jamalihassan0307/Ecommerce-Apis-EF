using Microsoft.EntityFrameworkCore;
using Ecommerce_Apis.CartModule.Models;
using Ecommerce_Apis.CartModule.DTOs;
using Ecommerce_Apis.CartModule.Repositories.InterFace;
using Ecommerce_Apis.Data;
using Ecommerce_Apis.CartModule.Helpers;

namespace Ecommerce_Apis.CartModule.Repositories
{
    public class CartRepositories : ICartRepositories
    {
        private readonly ApplicationDbContext _context;

        public CartRepositories(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateCart(AddToCartRequestDTO request, string userId)
        {
            // Validate input
            if (request.ProductId == null)
                throw new ArgumentException("ProductId in the request is null.");

            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            // Check if the product already exists in the user's cart
            bool productExists = await _context.Carts
                .AsNoTracking()
                .AnyAsync(cart => cart.ProductId == request.ProductId && cart.UserId == userId);

            if (productExists)
                return false;

            // Create and add a new cart entry
            var cart = new Cart
            {
                UserId = userId,
                ProductId = request.ProductId,
                CouponId = request.CouponId, // Ensure CouponId can be null or optional
                Quantity = request.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            _context.Carts.Add(cart);

            // Save changes to the database
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<List<CartItemDTO>> GetUserCart(string userId)
        {
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    Cart = c,
                    Product = c.Product,
                    Coupon = c.CouponId != 0 ? _context.Coupons.FirstOrDefault(cp => cp.Id == c.CouponId) : null
                })
                .ToListAsync();

            return cartItems.Select(item => new CartItemDTO
            {
                Id = item.Cart.Id,
                ProductId = item.Cart.ProductId,
                ProductName = item.Product.Name,
                OriginalPrice = item.Product.Price,
                DiscountedPrice = item.Coupon != null 
                    ? PriceCalculator.CalculateDiscountedPrice(item.Product.Price, item.Coupon.DiscountType, item.Coupon.Discount)
                    : item.Product.Price,
                Discount = item.Coupon?.Discount ?? 0,
                DiscountType = item.Coupon?.DiscountType ?? "NONE",
                CouponId = item.Cart.CouponId,
                ProductURL = item.Product.ProductURL,
                Quantity = item.Cart.Quantity,
                TotalPrice = item.Coupon != null 
                    ? PriceCalculator.CalculateTotal(
                        PriceCalculator.CalculateDiscountedPrice(item.Product.Price, item.Coupon.DiscountType, item.Coupon.Discount),
                        item.Cart.Quantity)
                    : PriceCalculator.CalculateTotal(item.Product.Price, item.Cart.Quantity),
                CreatedAt = item.Cart.CreatedAt,
                ProductImages = item.Product.ProductImages.Select(pi => pi.ImagePath).ToList()
            }).ToList();
        }

        public async Task<CartItemDTO> GetCartById(string userId, int cartId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(c => c.Id == cartId)
                .Select(c => new CartItemDTO
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                     ProductName = c.Product.Name,
        OriginalPrice = c.Product.Price,  // Changed from Price to OriginalPrice
        ProductURL = c.Product.ProductURL,
                    CouponId = c.CouponId,
                    Quantity = c.Quantity,
                    CreatedAt = c.CreatedAt,
                    ProductImages = c.Product.ProductImages.Select(pi => pi.ImagePath).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteCartItem(int cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart == null) return false;

            _context.Carts.Remove(cart);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> DeleteAllCartLists(string userId)
        {
            var wishLists = _context.WishLists.Where(list => list.UserId == userId);

            if (!wishLists.Any()) return false;

            _context.WishLists.RemoveRange(wishLists);

            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateCartItems(updateItem model)
        {
            
                var cart = await _context.Carts.FindAsync(model.CartId);
                if (cart != null)
                {
                    cart.Quantity = model.Quantity;
                    _context.Entry(cart).State = EntityState.Modified;
                }
            

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
