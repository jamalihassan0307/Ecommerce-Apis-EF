using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce_Apis.CouponModule.Models;
using Ecommerce_Apis.ProductModule.Models;

namespace Ecommerce_Apis.OrderModule.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int? CouponId { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual  Order Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("CouponId")]
        public virtual Coupon Coupon { get; set; }
    }
} 