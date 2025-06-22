using AKhderApi.Models;
using AKhderApi.Repositories;
using AutoMapper;
using Azure.Core;

namespace AKhderApi.Services
{
    public class CartService: ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }

        public async Task<(decimal totalPrice, decimal totalCarbonFootprin)> CalculateCartTotal()
        {
            var userCart = await _unitOfWork.Carts.FindAsync(c => c.Id == 1, new[] { "ProductCarts.Product" });

            if (userCart == null || !userCart.ProductCarts.Any())
                return (0, 0);


            var totalPrice = userCart.ProductCarts.Sum(pc =>
            {
                var productPrice = CalculateDiscountedPrice(pc.Product);
                return pc.Quantity * productPrice;
            });

            var totalCarbonFootprint = userCart.ProductCarts.Sum(pc => pc.Quantity * pc.Product.CarbonFootprint);

            return (Math.Round(totalPrice, 2), Math.Round(totalCarbonFootprint, 2));
        }
        public async Task<(decimal totalPrice, decimal totalCarbonFootprin, decimal totalWeight)> CalculateCartTotalwithWeight()
        {
            var userCart = await _unitOfWork.Carts.FindAsync(c => c.Id == 1, new[] { "ProductCarts.Product" });

            if (userCart == null || !userCart.ProductCarts.Any())
                return (0, 0,0);

            var totalPrice = userCart.ProductCarts.Sum(pc =>
            {
                var productPrice = CalculateDiscountedPrice(pc.Product);
                return pc.Quantity * productPrice;
            });

            var totalCarbonFootprint = userCart.ProductCarts.Sum(pc => pc.Quantity * pc.Product.CarbonFootprint);

            var totalWeight = userCart.ProductCarts.Sum(pc => pc.Quantity * (pc.Product.Weight ?? 0));

            return (Math.Round(totalPrice, 2), Math.Round(totalCarbonFootprint, 2), Math.Round(totalWeight, 4));
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


        public async Task ClearCart()
        {
            var userCart = await _unitOfWork.Carts.FindAsync(c => c.Id == 1, new[] { "ProductCarts.Product" });
            userCart.ProductCarts.Clear();

            await _unitOfWork.CompleteAsync();
        }

        public async Task<Cart?> GetCart()
        {
             var userCart =  await _unitOfWork.Carts.FindAsync(c => c.Id == 1 , new[] { "ProductCarts.Product" });
             return userCart;
        }

        public async Task<decimal> GetTotalWeight()
        {
            var userCart = await _unitOfWork.Carts.FindAsync(c => c.Id == 1, new[] { "ProductCarts.Product" });
            if (userCart == null || !userCart.ProductCarts.Any())
                return 0;

            var totalWeight = userCart.ProductCarts.Sum(pc => pc.Quantity * (pc.Product.Weight ?? 0));

            return Math.Round(totalWeight, 4);

        }


    }
}
