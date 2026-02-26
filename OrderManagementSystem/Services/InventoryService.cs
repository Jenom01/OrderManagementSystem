using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interfaces;
using OrderManagementSystem.Services.Interfaces;

namespace OrderManagementSystem.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly OrderManagementDbContext _context;
        private readonly IProductRepository _productRepo;

        public InventoryService(OrderManagementDbContext context, IProductRepository productRepo)
        {
            _context = context;
            _productRepo = productRepo;
        }

        public async Task ValidateStockAsync(List<OrderItem> items)
        {
            var productIds = items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var item in items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    throw new Exception($"Product with ID {item.ProductId} not found.");
                
                if (product.Stock < item.Quantity)
                    throw new Exception(
                        $"Insufficient stock for product {product.Name}. " +
                        $"Available: {product.Stock}, Requested: {item.Quantity}.");
            }
        }

        public async Task UpdateStockAsync(List<OrderItem> items)
        {
            var productIds = items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var item in items)
            {
                var product = products[item.ProductId];
                product.Stock -= item.Quantity;
            }

            await _context.SaveChangesAsync();
        }       
    }
}
