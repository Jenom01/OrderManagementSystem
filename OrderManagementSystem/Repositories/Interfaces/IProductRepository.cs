using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task AddProductAsync(Product product);
        void UpdateProduct(Product product);
    }
}
