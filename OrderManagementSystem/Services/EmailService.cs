using OrderManagementSystem.Models;
using OrderManagementSystem.Services.Interfaces;

namespace OrderManagementSystem.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendOrderStatusEmailAsync(string email, OrderStatus status)
        {
            await Task.Delay(200); // Simulate email sending delay

            Console.WriteLine(
                $"Email sent to {email} with order status: {status}");
        }
    }
}
