using OrderManagementSystem.Services.Interfaces;

namespace OrderManagementSystem.Services
{
    public class DiscountService : IDiscountService
    {
        public decimal ApplyDiscount(decimal totalAmount)
        {
            if (totalAmount > 200)
            {
                return totalAmount * 0.10m;
            }

            if (totalAmount > 100)
            {
                return totalAmount * 0.05m;
            }

            return 0;
        }
    }
}
