using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Apis.CouponModule.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        public decimal Discount { get; set; }

        [Required]
        [MaxLength(20)]
        public string DiscountType { get; set; }

        public DateTime ExpirationDate { get; set; }

        // Navigation properties
        public virtual ICollection<CouponProduct> CouponProducts { get; set; }
        public virtual ICollection<CouponCategory> CouponCategories { get; set; }
    }
} 