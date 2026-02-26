using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interfaces;
using OrderManagementSystem.Services.Interfaces;

namespace OrderManagementSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderManagementDbContext _context;

        private readonly IOrderRepository _orderRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IProductRepository _productRepo;

        private readonly IInventoryService _inventoryService;
        private readonly IDiscountService _discountService;
        private readonly IPaymentService _paymentService;
        private readonly IInvoiceService _invoiceService;

        public OrderService(
            OrderManagementDbContext context,
            IOrderRepository orderRepo,
            ICustomerRepository customerRepo,
            IProductRepository productRepo,
            IInventoryService inventoryService,
            IDiscountService discountService,
            IPaymentService paymentService,
            IInvoiceService invoiceService)
        {
            _context = context;
            _orderRepo = orderRepo;
            _customerRepo = customerRepo;
            _productRepo = productRepo;
            _inventoryService = inventoryService;
            _discountService = discountService;
            _paymentService = paymentService;
            _invoiceService = invoiceService;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "Order cannot be null.");

            if (order.OrderItems == null || !order.OrderItems.Any())
                throw new InvalidOperationException("Order must have at least one item.");

            if (order.CustomerId <= 0)
                throw new InvalidOperationException("Invalid customer.");

            var customer = await _customerRepo.GetCustomerByIdAsync(order.CustomerId);

            if (customer == null)
                throw new InvalidOperationException("Customer not found.");

            decimal totalAmount = 0m;

            foreach (var item in order.OrderItems)
            {
                var product = await _productRepo.GetProductByIdAsync(item.ProductId);

                if (product == null)
                    throw new InvalidOperationException($"Product with ID {item.ProductId} not found.");

                if (item.Quantity <= 0)
                    throw new InvalidOperationException($"Invalid quantity for product {product.Name}.");

                if (product.Stock < item.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for product {product.Name}. " +
                        $"Available: {product.Stock}, Requested: {item.Quantity}");

                item.UnitPrice = product.Price;

                totalAmount += item.UnitPrice * item.Quantity;
            }

            await _inventoryService.ValidateStockAsync(order.OrderItems.ToList());

            var discount = _discountService.ApplyDiscount(totalAmount);
            totalAmount -= discount;

            if (totalAmount <= 0)
                throw new InvalidOperationException("Total amount must be greater than zero after applying discounts.");

            order.TotalAmount = totalAmount;

            var paymentSuccessful = await _paymentService
                .ProcessPaymentAsync(order.PaymentMethod, totalAmount);

            if (!paymentSuccessful)
            {
                throw new InvalidOperationException("Payment failed.");
            }

            IDbContextTransaction? transaction = null;

            try
            {
                if (_context.Database.IsRelational())
                    transaction = await _context.Database.BeginTransactionAsync();

                await _orderRepo.AddOrderAsync(order);

                await _inventoryService.UpdateStockAsync(order.OrderItems.ToList());

                await _invoiceService.GenerateInvoiceAsync(order);

                await _context.SaveChangesAsync();

                if (transaction != null)
                    await transaction.CommitAsync();
            }
            catch
            {
                if (transaction != null)
                    await transaction.RollbackAsync();

                throw;
            }

            return order;
        }
    }
}
