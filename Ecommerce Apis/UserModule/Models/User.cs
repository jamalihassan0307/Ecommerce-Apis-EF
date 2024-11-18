using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce_Apis.UserModule.Constants;

namespace Ecommerce_Apis.UserModule.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(255)]
        public string Image { get; set; }

        public int RoleId { get; set; }

        [NotMapped]
        public string RoleName => RoleConstants.GetRoleName(RoleId);
    }
} 