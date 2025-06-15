using AKhderApi.Repositories;
using AKhderApi.Services;
using Stripe.Checkout;

namespace SmartCartCarbonFootprintApi.Services
{
    public class StripeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;

        public StripeService(IUnitOfWork unitOfWork, ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
        }

        public async Task<string> CreateCheckoutSession(string userId)
        {
            var (totalPrice, totalCarbonFootprint) = await _cartService.CalculateCartTotal(userId);

            if (totalPrice == 0)
                throw new Exception("Cart is empty or does not exist");

            long totalAmount = (long)(totalPrice * 100);

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
                SuccessUrl = "https://localhost:7008/api/Stripe/payment-success?userId=" + userId,
                CancelUrl = "https://localhost:7008/api/Stripe/payment-cancel"
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            return session.Url;
        }
    }
}
