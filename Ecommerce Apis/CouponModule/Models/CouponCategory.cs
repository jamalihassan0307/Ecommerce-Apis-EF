using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce_Apis.ProductModule.Models;

namespace Ecommerce_Apis.CouponModule.Models
{
    public class CouponCategory
    {
        public int CouponId { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("CouponId")]
        public virtual Coupon Coupon { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
} 