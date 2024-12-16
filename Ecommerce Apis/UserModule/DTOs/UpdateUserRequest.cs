namespace Ecommerce_Apis.UserModule.DTOs
{
    public class UpdateUserRequest
    {
        public string FullName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }


        public string oldPassword { get; set; }

        public string PasswordHash { get; set; }
        public IFormFile? Image { get; set; }
    }
        public class UpdateUserByAdminRequest
    {
        public int id { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile? Image { get; set; }
        public int RoleId { get; set; }
    }

}
