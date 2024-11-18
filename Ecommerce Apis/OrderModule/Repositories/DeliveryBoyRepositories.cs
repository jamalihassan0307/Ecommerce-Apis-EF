using Ecommerce_Apis.Data;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Apis.OrderModule.DTOs;
using Ecommerce_Apis.OrderModule.Models;
using Ecommerce_Apis.OrderModule.Repositories.InterFace;

namespace Ecommerce_Apis.OrderModule.Repositories
{
    public class DeliveryBoyRepositories : IDeliveryBoyRepositories
    {
        private readonly ApplicationDbContext _context;

        public DeliveryBoyRepositories(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AssignDelivery(string deliveryBoyId, int orderId)
        {
            var assignment = new DeliveryAssignment
            {
                OrderId = orderId,
                DeliveryBoyId = deliveryBoyId,
                AssignedAt = DateTime.UtcNow,
                Status = "Assigned"
            };

            _context.DeliveryAssignments.Add(assignment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<DeliveryOrderDTO>> GetAssignedOrders(string deliveryBoyId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.DeliveryAssignment)
                .Where(o => o.DeliveryAssignment.DeliveryBoyId == deliveryBoyId)
                .Select(o => new DeliveryOrderDTO
                {
                    OrderId = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    City = o.City,
                    Street = o.Street,
                    PostalCode = o.PostalCode,
                    Region = o.Region,
                    FullName = o.User.FullName
                })
                .ToListAsync();
        }

        public async Task<DeliveryOrderDTO> GetOrderDetails(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.DeliveryAssignment)
                .Where(o => o.Id == orderId)
                .Select(o => new DeliveryOrderDTO
                {
                    OrderId = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    City = o.City,
                    Street = o.Street,
                    PostalCode = o.PostalCode,
                    Region = o.Region,
                    FullName = o.User.FullName
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateOrderStatus(int orderId, string status)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var assignment = await _context.DeliveryAssignments
                    .FirstOrDefaultAsync(d => d.OrderId == orderId);
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (assignment == null || order == null)
                    return false;

                assignment.Status = status;
                order.Status = status;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> MarkOrderAsDelivered(int orderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var assignment = await _context.DeliveryAssignments
                    .FirstOrDefaultAsync(d => d.OrderId == orderId);
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (assignment == null || order == null)
                    return false;

                assignment.Status = "Delivered";
                order.Status = "Delivered";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
