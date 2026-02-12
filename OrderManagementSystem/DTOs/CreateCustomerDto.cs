using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.DTOs
{
    public class CreateCustomerDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
