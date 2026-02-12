using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services.Interfaces
{
    public interface IInventoryService
    {
        Task ValidateStockAsync(List<OrderItem> items);
        Task UpdateStockAsync(List<OrderItem> items);
    }
}
