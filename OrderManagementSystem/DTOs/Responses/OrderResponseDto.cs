using OrderManagementSystem.Models;

namespace OrderManagementSystem.DTOs.Responses
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime OrderDate { get; set; }

        public CustomerResponseDto Customer { get; set; } = null!;
        public List<OrderItemResponseDto> OrderItems { get; set; } = new();
    }
}
