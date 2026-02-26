namespace OrderManagementSystem.DTOs.Responses
{
    public class InvoiceResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}
