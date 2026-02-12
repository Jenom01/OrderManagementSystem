namespace OrderManagementSystem.Services.Interfaces
{
    public interface IDiscountService
    {
        decimal ApplyDiscount(decimal totalAmount);
    }
}
