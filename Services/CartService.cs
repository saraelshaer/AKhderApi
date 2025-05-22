using AKhderApi.Models;
using AKhderApi.Repositories;
using AutoMapper;
using Azure.Core;

namespace AKhderApi.Services
{
    public class CartService: ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly decimal _tolerance;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            if (!decimal.TryParse(configuration["WeightSettings:Tolerance"], out _tolerance))
            {
                _tolerance = 0.1m; 
            }
        }

        public async Task<(decimal totalPrice, decimal totalCarbonFootprint)> CalculateCartTotal(string userId)
        {
            var userCart = await _unitOfWork.Carts.FindAsync(w => w.UserId == userId, new[] { "ProductCarts.Product" });

            if (userCart == null || !userCart.ProductCarts.Any())
                return (0, 0);

            var now = DateOnly.FromDateTime(DateTime.Now);

            var totalPrice = userCart.ProductCarts.Sum(pc =>
            {
                var productPrice = pc.Product.Price;
                if (pc.Product.DiscountId != null && pc.Product.Discount.ExpiryDate >= now)
                {
                    productPrice = productPrice * (1 - pc.Product.Discount.Percentage / 100);
                }
                return pc.Quantity * productPrice;
            });

            var totalCarbonFootprint = userCart.ProductCarts.Sum(pc => pc.Quantity * pc.Product.CarbonFootprint);

            return (Math.Round(totalPrice, 2), Math.Round(totalCarbonFootprint, 2));
        }

        public decimal CalculateDiscountedPrice(Product product)
        {
            var now = DateOnly.FromDateTime(DateTime.Now);

            if (product.DiscountId != null && product.Discount.ExpiryDate >= now)
            {
                return product.Price * (1 - product.Discount.Percentage / 100);
            }

            return product.Price;
        }

        public async Task UpdateCartTotals(Cart userCart, Product product, int quantity)
        {
            var productPrice = CalculateDiscountedPrice(product);

            userCart.TotalPrice += product.Price * quantity;
            userCart.TotalWeight += (product.Weight.HasValue ? product.Weight.Value : 0) * quantity;
            userCart.TotalCarbonFootprint += product.CarbonFootprint * quantity;

            await _unitOfWork.CompleteAsync();
        }

        public async Task ClearCart(Cart userCart)
        {
            userCart.TotalPrice = 0;
            userCart.TotalWeight = 0;
            userCart.TotalCarbonFootprint = 0;
            userCart.ProductCarts.Clear();

            await _unitOfWork.CompleteAsync();
        }

        public async Task<Cart?> GetCartByUserId(string userId)
        {
             var userCart =  await _unitOfWork.Carts.FindAsync(w => w.UserId == userId, new[] { "ProductCarts.Product" });
             return userCart;
        }

        public async Task<bool> CheckWeight(int cartId, decimal weight)
        {
            var userCart = await _unitOfWork.Carts.FindAsync(c => c.Id == cartId, new[] { "ProductCarts.Product" });
            if (userCart == null)
            {
                return false;
            }
            var expectedWeight = userCart.ProductCarts.Sum(
                pc => pc.Quantity * (pc.Product.Weight.HasValue ? pc.Product.Weight.Value : 0));

            if (Math.Abs(expectedWeight - weight) <= _tolerance) 
            {
                return true;
            }
            return false;
        }
    }
}
