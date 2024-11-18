using Microsoft.EntityFrameworkCore;
using Ecommerce_Apis.CartModule.Models;
using Ecommerce_Apis.CartModule.DTOs;
using Ecommerce_Apis.CartModule.Repositories.InterFace;
using Ecommerce_Apis.Data;

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
            var cart = new Cart
            {
                UserId = userId,
                ProductId = request.ProductId,
                CouponId = request.CouponId,
                Quantity = request.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            _context.Carts.Add(cart);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<CartItemDTO>> GetUserCart(string userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(c => c.UserId == userId)
                .Select(c => new CartItemDTO
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    Price = c.Product.Price.ToString(),
                    ProductURL = c.Product.ProductURL,
                    CouponId = c.CouponId,
                    Quantity = c.Quantity,
                    CreatedAt = c.CreatedAt,
                    ProductImages = c.Product.ProductImages.Select(pi => pi.ImagePath).ToList()
                })
                .ToListAsync();
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
                    Price = c.Product.Price.ToString(),
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

        public async Task<bool> DeleteAllUserCart(string userId)
        {
            var carts = await _context.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!carts.Any()) return false;

            _context.Carts.RemoveRange(carts);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCartItems(UpdateCartRequestDTO model)
        {
            foreach (var item in model.Items)
            {
                var cart = await _context.Carts.FindAsync(item.CartId);
                if (cart != null)
                {
                    cart.Quantity = item.Quantity;
                    _context.Entry(cart).State = EntityState.Modified;
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
