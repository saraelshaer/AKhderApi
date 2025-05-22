using AKhderApi.Models;

namespace AKhderApi.Services
{
    public interface ICartService
    {
        decimal CalculateDiscountedPrice(Product product);
        Task<(decimal totalPrice, decimal totalCarbonFootprin)> CalculateCartTotal(string userId);
        Task ClearCart(Cart userCart);
        Task<Cart?> GetCartByUserId(string userId);
        Task<bool> CheckWeight(int cartId, decimal weight);

    }
}
