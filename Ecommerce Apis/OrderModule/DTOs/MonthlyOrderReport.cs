namespace Ecommerce_Apis.OrderModule.DTOs
{
    public class MonthlyOrderReport
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
    }

}
