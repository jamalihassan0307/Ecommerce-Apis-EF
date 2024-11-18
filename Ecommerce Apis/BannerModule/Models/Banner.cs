using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Apis.BannerModule.Models
{
    public class Banner
    {
        [Key]
        public int Id { get; set; }

        public int LinkId { get; set; }
        
        public int Link { get; set; }
        
        public int CouponId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Image { get; set; }
    }
} 