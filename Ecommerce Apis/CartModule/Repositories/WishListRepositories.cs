using Microsoft.EntityFrameworkCore;
using Ecommerce_Apis.CartModule.Models;
using Ecommerce_Apis.CartModule.DTOs;
using Ecommerce_Apis.CartModule.Repositories.InterFace;
using Ecommerce_Apis.Data;

namespace Ecommerce_Apis.CartModule.Repositories
{
    public class WishListRepositories : IWishListRepositories
    {
        private readonly ApplicationDbContext _context;

        public WishListRepositories(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateWishList(AddToCartRequestDTO request, string userId)
        {
            var wishList = new WishList
            {
                UserId = userId,
                ProductId = request.ProductId,
                CouponId = request.CouponId,
                Quantity = request.Quantity
            };

            _context.WishLists.Add(wishList);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<CartItemDTO>> GetUserWishList(string userId)
        {
            return await _context.WishLists
                .Include(w => w.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(w => w.UserId == userId)
                .Select(w => new CartItemDTO
                {
                    Id = w.Id,
                    ProductId = w.ProductId,
                    ProductName = w.Product.Name,
                    Price = w.Product.Price.ToString(),
                    ProductURL = w.Product.ProductURL,
                    CouponId = w.CouponId,
                    Quantity = w.Quantity,
                    CreatedAt = w.CreatedAt,
                    ProductImages = w.Product.ProductImages.Select(pi => pi.ImagePath).ToList()
                })
                .ToListAsync();
        }

        public async Task<CartItemDTO> GetWishListById(string userId, int wishlistId)
        {
            return await _context.WishLists
                .Include(w => w.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(w => w.Id == wishlistId && w.UserId == userId)
                .Select(w => new CartItemDTO
                {
                    Id = w.Id,
                    ProductId = w.ProductId,
                    ProductName = w.Product.Name,
                    Price = w.Product.Price.ToString(),
                    ProductURL = w.Product.ProductURL,
                    CouponId = w.CouponId,
                    Quantity = w.Quantity,
                    CreatedAt = w.CreatedAt,
                    ProductImages = w.Product.ProductImages.Select(pi => pi.ImagePath).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteWishListItem(int wishlistId)
        {
            var wishlist = await _context.WishLists.FindAsync(wishlistId);
            if (wishlist == null) return false;

            _context.WishLists.Remove(wishlist);
            return await _context.SaveChangesAsync() > 0;
        }

      

        public async Task<bool> UpdateWishLists(updateItem model)
        {
           
                var wishlist = await _context.WishLists.FindAsync(model.CartId);
                if (wishlist != null)
                {
                    wishlist.Quantity = model.Quantity;
                }
          

            return await _context.SaveChangesAsync() > 0;
        }

        // Implement other methods similarly to CartRepository...
    }
}
