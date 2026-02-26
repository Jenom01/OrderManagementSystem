using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.DTOs.Responses;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interfaces;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly OrderManagementDbContext _context;
        private readonly IProductRepository _productRepo;

        public ProductController(OrderManagementDbContext context, IProductRepository productRepo)
        {
            _context = context;
            _productRepo = productRepo;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productRepo.GetAllProductsAsync();

            var response = products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepo.GetProductByIdAsync(id);

            if (product == null)
                return NotFound();

            var response = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            };

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]        
        public async Task<IActionResult> Add(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock
            };

            await _productRepo.AddProductAsync(product);
            await _context.SaveChangesAsync();

            var response = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            };

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateProductDto dto)
        {
            var product = await _productRepo.GetProductByIdAsync(id);

            if (product == null)
                return NotFound();

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Stock = dto.Stock;

            _productRepo.UpdateProduct(product);
            await _context.SaveChangesAsync();

            var response = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            };

            return Ok(response);
        }
    }
}
