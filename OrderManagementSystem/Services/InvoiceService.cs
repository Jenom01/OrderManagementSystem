using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interfaces;
using OrderManagementSystem.Services.Interfaces;

namespace OrderManagementSystem.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IOrderRepository _orderRepo;

        public InvoiceService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<Invoice> GenerateInvoiceAsync(Order order)
        {
            if (order.TotalAmount <= 0)
                throw new InvalidOperationException("Cannot generate invoice for an order with zero or negative total amount.");

            var invoice = new Invoice
            {
                OrderId = order.Id,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = order.TotalAmount
            };

            order.Invoice = invoice;

            _orderRepo.UpdateOrder(order);
            await _orderRepo.SaveChangesAsync();

            return invoice;
        }
    }
}
