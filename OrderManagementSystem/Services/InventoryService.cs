using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interfaces;
using OrderManagementSystem.Services.Interfaces;

namespace OrderManagementSystem.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IProductRepository _productRepo;

        public InventoryService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task ValidateStockAsync(List<OrderItem> items)
        {
            foreach (var item in items)
            {
                var product = await _productRepo.GetProductByIdAsync(item.ProductId);

                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found.");
                
                if (product.Stock < item.Quantity)
                    throw new Exception($"Insufficient stock for product {product.Name}. Requested: {item.Quantity}, Available: {product.Stock}");
            }
        }

        public async Task UpdateStockAsync(List<OrderItem> items)
        {
            foreach (var item in items)
            {
                var product = await _productRepo.GetProductByIdAsync(item.ProductId);

                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found.");

                product.Stock -= item.Quantity;
                _productRepo.UpdateProduct(product);
            }

            await _productRepo.SaveChangesAsync();
        }       
    }
}
