using Microsoft.EntityFrameworkCore;
using Ecommerce_Apis.OrderModule.DTOs;
using Ecommerce_Apis.OrderModule.Models;
using Ecommerce_Apis.OrderModule.Repositories.InterFace;
using AutoMapper;
using Ecommerce_Apis.Data;
using OrderItem = Ecommerce_Apis.OrderModule.Models.OrderItem;

namespace Ecommerce_Apis.OrderModule.Repositories
{
    public class OrderRepositories : IOrderRepositories
    {
        private readonly ApplicationDbContext _context;

        public OrderRepositories(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<dynamic> CreateOrder(CreateOrderRequest request, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    City = request.City,
                    Street = request.Street,
                    PostalCode = request.PostalCode,
                    Region = request.Region,
                    Status = "Pending",
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = 0
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                decimal totalAmount = 0;
                foreach (var item in request.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null) 
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = product.Price,
                        CouponId = item.CouponId,
                        Order = order
                    };

                    _context.OrderItems.Add(orderItem);
                    totalAmount += product.Price * item.Quantity;
                }

                order.TotalAmount = totalAmount;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CreateOrder("Order Created Successfully", order.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<bool> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<dynamic> GetOrdersByUserId(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(o => o.UserId == userId)
                .Select(o => new OrderResponse
                {
                    OrderId = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    OrderDate = o.OrderDate,
                    City = o.City,
                    Street = o.Street,
                    PostalCode = o.PostalCode,
                    Region = o.Region,
                    Items = o.OrderItems.Select(oi => new OrderItemDTO
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        ProductName = oi.Product.Name,
                        ProductURL = oi.Product.ProductURL,
                        ProductImagePath = oi.Product.ProductImages.FirstOrDefault().ImagePath
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<dynamic> CancelOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return null;

            order.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<dynamic> GetOrdersByStatus(string status)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Status == status)
                .Select(o => new OrderResponse
                {
                    OrderId = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    OrderDate = o.OrderDate,
                    City = o.City,
                    Street = o.Street,
                    PostalCode = o.PostalCode,
                    Region = o.Region
                })
                .ToListAsync();
        }

        public async Task<dynamic> DeleteOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return null;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<OrderSummary> GetOrderSummary()
        {
            var orders = await _context.Orders.ToListAsync();
            return new OrderSummary
            {
                TotalOrders = orders.Count,
                TotalSales = orders.Sum(o => o.TotalAmount)
            };
        }

        public async Task<MonthlyOrderReport> GetMonthlyOrderReport()
        {
            var now = DateTime.UtcNow;
            var orders = await _context.Orders
                .Where(o => o.OrderDate.Month == now.Month && o.OrderDate.Year == now.Year)
                .ToListAsync();

            return new MonthlyOrderReport
            {
                Year = now.Year,
                Month = now.Month,
                TotalOrders = orders.Count,
                TotalSales = orders.Sum(o => o.TotalAmount)
            };
        }

        public async Task<OverallOrderReport> GetOverallOrderReport()
        {
            var orders = await _context.Orders.ToListAsync();
            var totalOrders = orders.Count;
            var totalSales = orders.Sum(o => o.TotalAmount);

            return new OverallOrderReport
            {
                TotalOrders = totalOrders,
                TotalSales = totalSales,
                AverageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0,
                MinimumOrderValue = orders.Any() ? orders.Min(o => o.TotalAmount) : 0,
                MaximumOrderValue = orders.Any() ? orders.Max(o => o.TotalAmount) : 0
            };
        }

        public async Task<OrderStatusCount> GetOrderStatusCounts()
        {
            var orders = await _context.Orders.ToListAsync();
            return new OrderStatusCount
            {
                Status = "All",
                TotalCount = orders.Count
            };
        }

        public async Task<List<OrderUserIDDTOS>> GetAllOrders(IMapper mapper)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Select(o => new OrderUserIDDTOS
                {
                    OrderId = o.Id,
                    UserId = int.Parse(o.UserId),
                    fullname = o.User.FullName,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    City = o.City,
                    Street = o.Street,
                    PostalCode = o.PostalCode,
                    Region = o.Region,
                    Items = o.OrderItems.Select(oi => new OrderItemDTO
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        ProductName = oi.Product.Name,
                        ProductURL = oi.Product.ProductURL
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderDTOS> GetOrderById(int orderId, IMapper mapper)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return null;

            return new OrderDTOS
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                City = order.City,
                Street = order.Street,
                PostalCode = order.PostalCode,
                Region = order.Region,
                Items = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    ProductName = oi.Product.Name,
                    ProductURL = oi.Product.ProductURL,
                    ProductImagePath = oi.Product.ProductImages.FirstOrDefault()?.ImagePath
                }).ToList()
            };
        }
    }
}
