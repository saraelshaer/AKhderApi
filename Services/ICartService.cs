using AKhderApi.Models;

namespace AKhderApi.Services
{
    public interface ICartService
    {
        decimal CalculateDiscountedPrice(Product product);
        Task UpdateCartTotals(Cart userCart, Product product, int quantity);
        Task<(decimal totalPrice, decimal totalCarbonFootprint)> CalculateCartTotal(string userId);
        Task ClearCart(Cart userCart);
    }
}
