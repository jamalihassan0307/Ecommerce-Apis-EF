namespace Ecommerce_Apis.OrderModule.DTOs
{
    public class UpdateAddressDTO
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
    }
} 