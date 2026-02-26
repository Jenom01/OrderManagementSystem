using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.DTOs.Responses;
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
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int customerId))
                return Unauthorized();

            var order = new Order
            {
                CustomerId = customerId,
                PaymentMethod = dto.PaymentMethod,
                OrderItems = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                }).ToList()
            };

            var createdOrder = await _orderService.CreateOrderAsync(order);

            var response = new OrderResponseDto
            {
                Id = createdOrder.Id,
                OrderDate = createdOrder.OrderDate,
                TotalAmount = createdOrder.TotalAmount,
                Status = createdOrder.Status,
                Customer = new CustomerResponseDto
                {
                    Id = createdOrder.Customer!.Id,
                    Name = createdOrder.Customer.Name,
                    Email = createdOrder.Customer.Email
                },
                OrderItems = createdOrder.OrderItems!.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product!.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                }).ToList()
            };

            return Ok(response);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderRepo.GetOrderByIdAsync(id);

            if (order == null)
                return NotFound();

            var response = new OrderResponseDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Customer = new CustomerResponseDto
                {
                    Id = order.Customer!.Id,
                    Name = order.Customer.Name,
                    Email = order.Customer.Email
                },
                OrderItems = order.OrderItems!.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product!.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                }).ToList()
            };

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderRepo.GetAllOrdersAsync();

            var response = orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Customer = new CustomerResponseDto
                {
                    Id = order.Customer!.Id,
                    Name = order.Customer.Name,
                    Email = order.Customer.Email
                },
                OrderItems = order.OrderItems!.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product!.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                }).ToList()
            }).ToList();

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _orderRepo.GetOrderByIdAsync(id);

            if (order == null)
                return NotFound();

            order.Status = status;

            _orderRepo.UpdateOrder(order);

            var customer = await _customerRepo.GetCustomerByIdAsync(order.CustomerId);

            if (customer != null)
            {
                await _emailService.SendOrderStatusEmailAsync(customer.Email!, status);
            }

            var response = new OrderResponseDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Customer = new CustomerResponseDto
                {
                    Id = customer!.Id,
                    Name = customer.Name,
                    Email = customer.Email
                },
                OrderItems = order.OrderItems!.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product!.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                }).ToList()
            };

            return Ok(response);
        }
    }
}
