namespace OrderManagementSystem.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderStatusEmailAsync(string email, string status);
    }
}
