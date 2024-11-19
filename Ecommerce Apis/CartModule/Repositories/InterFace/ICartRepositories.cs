using Ecommerce_Apis.CartModule.DTOs;
using Ecommerce_Apis.ProductModule.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_Apis.CartModule.Repositories.InterFace
{
    public interface ICartRepositories

    {
        Task<bool> CreateCart(AddToCartRequestDTO request, string id);
        Task<List<CartItemDTO>> GetUserCart(string userId);
        Task<CartItemDTO> GetCartById(string userId, int cartId);
        Task<bool> DeleteCartItem(int cartId);
        Task<bool> UpdateCartItems(updateItem model);
    }
}
