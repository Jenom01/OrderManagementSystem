using Moq;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interfaces;
using OrderManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementSystem.Tests.Services
{
    public class InventoryServiceTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly InventoryService _service;

        public InventoryServiceTests()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _service = new InventoryService(_productRepoMock.Object);
        }

        [Fact]
        public async Task ValidateStock_ShouldThrow_WhenProductNotFound()
        {
            _productRepoMock
                .Setup(repo => repo.GetProductByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Product?)null);

            var items = new List<OrderItem>
            {
                new() { ProductId = 1, Quantity = 1 }
            };

            await Assert.ThrowsAsync<Exception>(() => _service.ValidateStockAsync(items));
        }

        [Fact]
        public async Task ValidateStock_ShouldThrow_WhenInsufficientStock()
        {
            _productRepoMock
                .Setup(repo => repo.GetProductByIdAsync(1))
                .ReturnsAsync(new Product { Id = 1, Stock = 2, Name = "Test" });

            var items = new List<OrderItem>
            {
                new() { ProductId = 1, Quantity = 5 }
            };

            await Assert.ThrowsAsync<Exception>(() => _service.ValidateStockAsync(items));
        }

        [Fact]
        public async Task ValidateStock_ShouldPass_WhenSufficientStock()
        {
            _productRepoMock
                .Setup(repo => repo.GetProductByIdAsync(1))
                .ReturnsAsync(new Product { Id = 1, Stock = 10 });

            var items = new List<OrderItem>
            {
                new() { ProductId = 1, Quantity = 2 }
            };

            await _service.InvokationShouldNotThrowAsync(items);
        }
    }

    static class InventoryServiceTestExtensions
    {
        public static async Task InvokationShouldNotThrowAsync(this InventoryService service, List<OrderItem> items)
        {
            await service.ValidateStockAsync(items);
        }
    }
}
