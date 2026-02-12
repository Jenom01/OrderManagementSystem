namespace OrderManagementSystem.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(string paymentMethod, decimal amount);
    }
}
