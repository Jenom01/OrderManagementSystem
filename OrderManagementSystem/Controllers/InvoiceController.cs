using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs.Responses;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    [Authorize(Roles = "Admin")]
    public class InvoiceController : ControllerBase
    {
        private readonly OrderManagementDbContext _context;
        public InvoiceController(OrderManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _context.Invoices.ToListAsync();

            var response = invoices.Select(i => new InvoiceResponseDto
            {
                Id = i.Id,
                OrderId = i.OrderId,
                TotalAmount = i.TotalAmount,
                InvoiceDate = i.InvoiceDate
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var invoice = await _context.Invoices.Include(i => i.Order)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            var response = new InvoiceResponseDto
            {
                Id = invoice.Id,
                OrderId = invoice.OrderId,
                TotalAmount = invoice.TotalAmount,
                InvoiceDate = invoice.InvoiceDate
            };

            return Ok(response);
        }
    }
}
