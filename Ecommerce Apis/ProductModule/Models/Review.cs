using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce_Apis.UserModule.Models;

namespace Ecommerce_Apis.ProductModule.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        [Range(0, 5)]
        public double Points { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Add these properties to match the mapping
        public string ProductURL { get; set; }
        public string Fullname { get; set; }
        public string Image { get; set; }
    }
}
