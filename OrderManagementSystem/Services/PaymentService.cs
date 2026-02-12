using OrderManagementSystem.Services.Interfaces;

namespace OrderManagementSystem.Services
{
    public class PaymentService : IPaymentService
    {
        public async Task<bool> ProcessPaymentAsync(string paymentMethod, decimal amount)
        {
            await Task.Delay(500); // Simulate payment processing delay

            if (paymentMethod == "CreditCard" || paymentMethod == "PayPal")
                return true;

            return false;
        }
    }
}
