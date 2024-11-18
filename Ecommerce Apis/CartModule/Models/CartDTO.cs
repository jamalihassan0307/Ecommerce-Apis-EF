using Ecommerce_Apis.CartModule.DTOs;

namespace Ecommerce_Apis.CartModule.Models
{
    public class CartDTO
    {
        public int UserId { get; set; }
        public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
    }
}
