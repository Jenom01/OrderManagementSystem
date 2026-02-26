using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.DTOs.Responses;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interfaces;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly OrderManagementDbContext _context;
        private readonly ICustomerRepository _customerRepo;

        public CustomerController(OrderManagementDbContext context, ICustomerRepository customerRepo)
        {
            _context = context;
            _customerRepo = customerRepo;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CreateCustomerDto dto)
        {
            var customer = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
            };

            await _customerRepo.AddCustomerAsync(customer);
            await _context.SaveChangesAsync();

            var response = new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email
            };

            return Ok(response);
        }

        [HttpGet("{id}/orders")]
        public async Task<IActionResult> GetCustomerOrders(int id)
        {
            var customer = await _customerRepo.GetCustomerByIdAsync(id);

            if (customer == null)
                return NotFound();

            var response = customer.Orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                Customer = new CustomerResponseDto
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Email = customer.Email
                },
                OrderItems = o.OrderItems!.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product!.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                }).ToList()
            }).ToList();

            return Ok(response);
        }
    }
}