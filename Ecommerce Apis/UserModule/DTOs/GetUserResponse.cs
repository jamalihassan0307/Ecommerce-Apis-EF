﻿namespace Ecommerce_Apis.UserModule.DTOs
{
    public class GetUserResponse
    {

        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public int RoleId { get; set; }
        public string PasswordHash { get; set; }
        public string Image { get; set; }



    }
}
