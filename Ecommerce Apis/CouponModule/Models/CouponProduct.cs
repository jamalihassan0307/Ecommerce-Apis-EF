using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce_Apis.ProductModule.Models;

namespace Ecommerce_Apis.CouponModule.Models
{
    public class CouponProduct
    {
        public int CouponId { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("CouponId")]
        public virtual Coupon Coupon { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
} 