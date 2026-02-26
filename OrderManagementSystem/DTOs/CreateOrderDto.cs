using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.DTOs
{
    public class CreateOrderDto
    {
        public string PaymentMethod { get; set; } = string.Empty;

        [MinLength(1)]
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
