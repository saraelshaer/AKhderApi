using AKhderApi.Repositories;
using Stripe.Checkout;

namespace SmartCartCarbonFootprintApi.Services
{
    public class StripeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StripeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> CreateCheckoutSession(string userId)
        {
            var cart = (await _unitOfWork.Carts.GetAllAsync(
                        c => c.User.Id == userId,
                        new[] { "ProductCarts.Product", "User" }
                    )).FirstOrDefault();



            if (cart == null || !cart.ProductCarts.Any())
                throw new Exception("Cart is empty or does not exist");

            long totalAmount = (long)(cart.ProductCarts.Sum(cp => cp.Product.Price) * 100);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                   new()
                   {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = totalAmount,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Total Order"
                            }
                        },
                        Quantity = 1
                   }
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:7008/api/Stripe/payment-success",
                CancelUrl = "https://localhost:7008/api/Stripe/payment-cancel"
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            return session.Url;
        }
    }
}
