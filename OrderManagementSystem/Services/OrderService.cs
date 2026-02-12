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
        private readonly IOrderRepository _orderRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IProductRepository _productRepo;
        private readonly IInventoryService _inventoryService;
        private readonly IDiscountService _discountService;
        private readonly IPaymentService _paymentService;
        private readonly IInvoiceService _invoiceService;

        private readonly OrderManagementDbContext _context;

        public OrderService(
            IOrderRepository orderRepo,
            ICustomerRepository customerRepo,
            IProductRepository productRepo,
            IInventoryService inventoryService,
            IDiscountService discountService,
            IPaymentService paymentService,
            IInvoiceService invoiceService,
            OrderManagementDbContext context)
        {
            _orderRepo = orderRepo;
            _customerRepo = customerRepo;
            _productRepo = productRepo;
            _inventoryService = inventoryService;
            _discountService = discountService;
            _paymentService = paymentService;
            _invoiceService = invoiceService;
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new Exception("Order cannot be null.");

            if (order.OrderItems == null || !order.OrderItems.Any())
                throw new Exception("Order must have at least one item.");

            if (order.CustomerId <= 0)
                throw new Exception("Invalid customer.");

            var customer = await _customerRepo.GetCustomerByIdAsync(order.CustomerId);

            if (customer == null)
                throw new Exception("Customer not found.");

            foreach (var item in order.OrderItems)
            {
                var product = await _productRepo.GetProductByIdAsync(item.ProductId);
                
                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found.");
                
                if (item.Quantity <= 0)
                    throw new Exception("Invalid quantity for product " + product.Name);

                if (product.Stock < item.Quantity)
                    throw new Exception($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {item.Quantity}");
            }

            await _inventoryService.ValidateStockAsync(order.OrderItems!.ToList());

            decimal totalAmount = 0;

            //foreach (var item in order.OrderItems!)
            //{
            //    var product = await _productRepo.GetProductByIdAsync(item.ProductId);

            //    if (product == null)
            //        throw new Exception($"Product with ID {item.ProductId} not found.");

            //    item.UnitPrice = product.Price;

            //    totalAmount += item.Quantity * product.Price;
            //}

            var discount = _discountService.ApplyDiscount(totalAmount);
            totalAmount -= discount;

            if (totalAmount < 0)
                totalAmount = 0;

            order.TotalAmount = totalAmount;

            var paymentResult = await _paymentService.ProcessPaymentAsync(order.PaymentMethod!, totalAmount);

            if (!paymentResult)
            {
                throw new Exception("Payment failed.");
            }

            IDbContextTransaction? transaction = null;
                       
            try
            {
                if (_context.Database.IsRelational())
                    transaction = await _context.Database.BeginTransactionAsync();

                await _orderRepo.AddOrderAsync(order);
                await _orderRepo.SaveChangesAsync();

                await _inventoryService.UpdateStockAsync(order.OrderItems!.ToList());

                await _invoiceService.GenerateInvoiceAsync(order);

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
