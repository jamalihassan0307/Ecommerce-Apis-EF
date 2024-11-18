using Ecommerce_Apis.OrderModule.DTOs;

namespace Ecommerce_Apis.OrderModule.Repositories.InterFace
{
    public interface IDeliveryBoyRepositories
    {
        Task<bool> AssignDelivery(string deliveryBoyId, int orderId);
        Task<List<DeliveryOrderDTO>> GetAssignedOrders(string deliveryBoyId);
        Task<DeliveryOrderDTO> GetOrderDetails(int orderId);
        Task<bool> UpdateOrderStatus(int orderId, string status);
        Task<bool> MarkOrderAsDelivered(int orderId);
    }

}
