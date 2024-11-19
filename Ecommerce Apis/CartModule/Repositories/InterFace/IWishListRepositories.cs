﻿using Ecommerce_Apis.CartModule.DTOs;

namespace Ecommerce_Apis.CartModule.Repositories.InterFace
{
    public interface IWishListRepositories
    {
        Task<bool> CreateWishList(AddToCartRequestDTO request, string id);
        Task<List<CartItemDTO>> GetUserWishList(string userId);
        Task<CartItemDTO?> GetWishListById(string userId, int wishlistId);
        Task<bool> DeleteWishListItem(int wishlistId);
        Task<bool> UpdateWishLists(updateItem model);
    }
}
