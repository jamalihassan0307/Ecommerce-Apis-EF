using AutoMapper;
using Ecommerce_Apis.OrderModule.DTOs;

namespace Ecommerce_Apis.OrderModule.Repositories.InterFace
{
    public interface IOrderRepositories
    {
        Task<dynamic> CreateOrder(CreateOrderRequest request, string userid);
         Task<OrderDTOS> GetOrderById(int orderId, IMapper mapper);
        Task<bool> UpdateOrderStatus(int orderId, string status);
        Task<dynamic> GetOrdersByUserId(string userId);
        Task<dynamic> CancelOrder(int orderId);
        Task<dynamic> DeliveredlOrder(int orderId);
        Task<dynamic> DeleteOrder(int orderId);
        Task<List<OrderUserIDDTOS>> GetAllOrders(IMapper mapper);
    }
}
