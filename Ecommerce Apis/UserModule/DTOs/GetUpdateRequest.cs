namespace Ecommerce_Apis.UserModule.DTOs
{
    public class GetUpdateRequest
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Passwordhash { get; set; }
        public string oldPassword { get; set; }
        public string Image { get; set; }
    }
    public class GetUpdateByAdminRequest
    {
        public string Id { get; set; }
        public string FullName { get; set; } 
        public string PhoneNumber { get; set; }
        public string Image { get; set; }
        public int RoleId { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Oldpassword { get; set; }
        public string NewPassword { get; set; }
    } 
    public class ResetPasswordDTO
    {
        public string Id { get; set; }
        public string Passwordhash { get; set; }
    }
}
