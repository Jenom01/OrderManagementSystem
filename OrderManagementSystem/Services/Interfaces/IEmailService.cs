using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderStatusEmailAsync(string email, OrderStatus status);
    }
}
