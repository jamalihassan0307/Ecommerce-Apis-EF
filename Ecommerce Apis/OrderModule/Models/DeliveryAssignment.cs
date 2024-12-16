using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce_Apis.UserModule.Models;

namespace Ecommerce_Apis.OrderModule.Models
{
    public class DeliveryAssignment
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string DeliveryBoyId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [ForeignKey("DeliveryBoyId")]
        public virtual User DeliveryBoy { get; set; }
    }
} 