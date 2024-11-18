namespace Ecommerce_Apis.ProductModule.DTOs
{
    public class UpdateProductStockDTO
    {
        public int ProductId { get; set; }
        public int NewStockQuantity { get; set; }
    }
}
