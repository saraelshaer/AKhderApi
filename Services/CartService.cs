using AKhderApi.Repositories;
using AutoMapper;

namespace AKhderApi.Services
{
    public class CartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(decimal totalPrice, double totalCarbonFootprint)> CalculateCartTotal(string userId)
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

    }
}
