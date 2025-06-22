using AKhderApi.Models;
using System.Threading.Tasks;

namespace AKhderApi.Services
{
    public interface ICartService
    {
        decimal CalculateDiscountedPrice(Product product);
        Task<(decimal totalPrice, decimal totalCarbonFootprin)> CalculateCartTotal();
        Task ClearCart();
        Task<Cart?> GetCart();
        Task<(decimal totalPrice, decimal totalCarbonFootprin, decimal totalWeight)> CalculateCartTotalwithWeight();
        Task<decimal> GetTotalWeight();
    }
}
