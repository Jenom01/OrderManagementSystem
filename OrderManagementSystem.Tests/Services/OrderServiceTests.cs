using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interfaces;
using OrderManagementSystem.Services;
using OrderManagementSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementSystem.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepo = new();
        private readonly Mock<ICustomerRepository> _customerRepo = new();
        private readonly Mock<IProductRepository> _productRepo = new();
        private readonly Mock<IInventoryService> _inventory = new();
        private readonly Mock<IDiscountService> _discount = new();
        private readonly Mock<IPaymentService> _payment = new();
        private readonly Mock<IInvoiceService> _invoice = new();

        private readonly OrderManagementDbContext _context;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new OrderManagementDbContext(options);

            _service = new OrderService(
                _orderRepo.Object,
                _customerRepo.Object,
                _productRepo.Object,
                _inventory.Object,
                _discount.Object,
                _payment.Object,
                _invoice.Object,
                _context
            );
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldThrow_WhenNoItems()
        {
            var order = new Order
            {
                CustomerId = 1,
                OrderItems = new List<OrderItem>()
            };

            await Assert.ThrowsAsync<Exception>(() => _service.CreateOrderAsync(order));
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldThrow_WhenCustomerNotFound()
        {
            _customerRepo.Setup(r => r.GetCustomerByIdAsync(1))
                         .ReturnsAsync((Customer?)null);

            var order = new Order
            {
                CustomerId = 1,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 1 }
                }
            };

            await Assert.ThrowsAsync<Exception>(() => _service.CreateOrderAsync(order));
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldSucceed_WhenValid()
        {
            _customerRepo.Setup(r => r.GetCustomerByIdAsync(1))
                         .ReturnsAsync(new Customer { Id = 1 });

            _productRepo.Setup(r => r.GetProductByIdAsync(1))
                        .ReturnsAsync(new Product { Id = 1, Price = 10, Stock = 10 });

            _payment.Setup(p => p.ProcessPaymentAsync(It.IsAny<string>(), It.IsAny<decimal>()))
                    .ReturnsAsync(true);

            _discount.Setup(d => d.ApplyDiscount(It.IsAny<decimal>()))
                     .Returns(0);

            var order = new Order
            {
                CustomerId = 1,
                PaymentMethod = "CreditCard",
                OrderItems = new List<OrderItem>
                {
                    new() { ProductId = 1, Quantity = 1, UnitPrice = 10 }
                }
            };

            var result = await _service.CreateOrderAsync(order);
            
            result.Should().NotBeNull();
        }
    }
}
