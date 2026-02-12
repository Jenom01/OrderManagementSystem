using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interfaces;
using OrderManagementSystem.Services.Interfaces;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IEmailService _emailService;
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderService _orderService;        

        public OrderController(ICustomerRepository customerRepo, IEmailService emailService, IOrderRepository orderRepo, IOrderService orderService)
        {
            _customerRepo = customerRepo;
            _emailService = emailService;
            _orderRepo = orderRepo;
            _orderService = orderService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                PaymentMethod = dto.PaymentMethod,
                OrderItems = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }).ToList()
            };

            var createdOrder = await _orderService.CreateOrderAsync(order);

            return Ok(createdOrder);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderRepo.GetOrderByIdAsync(id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderRepo.GetAllOrdersAsync();
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _orderRepo.GetOrderByIdAsync(id);

            if (order == null)
                return NotFound();

            order.Status = status;

            _orderRepo.UpdateOrder(order);
            await _orderRepo.SaveChangesAsync();

            var customer = await _customerRepo.GetCustomerByIdAsync(order.CustomerId);

            if (customer != null)
            {
                await _emailService.SendOrderStatusEmailAsync(customer.Email!, status);
            }

            return Ok(order);
        }
    }
}
