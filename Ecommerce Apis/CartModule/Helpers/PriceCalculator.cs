namespace Ecommerce_Apis.CartModule.Helpers
{
    public static class PriceCalculator
    {
        public static decimal CalculateDiscountedPrice(decimal originalPrice, string discountType, decimal discount)
        {
            if (discount == 0) return originalPrice;

            return discountType == "PERCENTAGE"
                ? originalPrice * (1 - discount / 100)
                : Math.Max(originalPrice - discount, originalPrice * 0.7m);
        }

        public static decimal CalculateTotal(decimal price, int quantity)
        {
            return price * quantity;
        }
    }
} 