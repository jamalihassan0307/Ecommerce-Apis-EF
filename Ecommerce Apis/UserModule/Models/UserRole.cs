using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Apis.UserModule.Models
{
    public class UserRole
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
} 