using AKhderApi.Consts;
using AKhderApi.Models;
using AKhderApi.Repositories;
using AKhderApi.Services;

namespace SmartCartCarbonFootprintApi.Services
{
    public class OrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;

        public OrderService(IUnitOfWork unitOfWork, ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
        }

        public async Task<Order> CreateOrder(string userId)
        {
            var cart = await _cartService.GetCart();

            if (cart == null || !cart.ProductCarts.Any())
                throw new Exception("Cart is empty or does not exist");

            var totalPrice = cart.ProductCarts.Sum(pc => pc.Product.Price * pc.Quantity);
            var totalCarbonFootprint = cart.ProductCarts.Sum(pc => pc.Product.CarbonFootprint * pc.Quantity);

            var newOrder = new Order
            {
                UserId = userId,
                TotalPrice = totalPrice,
                TotalCarbonFootprint = totalCarbonFootprint,
                PaymentMethod = PaymentMethod.CreditCard,
                TransactionStatus = TransactionStatus.Completed
            };

            await _unitOfWork.Orders.AddAsync(newOrder);
            await _unitOfWork.CompleteAsync();

            foreach (var productCart in cart.ProductCarts)
            {
                var productOrder = new ProductOrder
                {
                    OrderId = newOrder.Id,
                    ProductId = productCart.ProductId,
                    Quantity = productCart.Quantity
                };

                await _unitOfWork.ProductOrders.AddAsync(productOrder);

                var product = await _unitOfWork.Products.FindAsync(p => p.Id == productCart.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= productCart.Quantity;
                    if (product.StockQuantity <= 0)
                        product.IsActive = false;

                    _unitOfWork.Products.Update(product);
                }
            }

            _unitOfWork.ProductCarts.RemoveRange(cart.ProductCarts);
            await _unitOfWork.CompleteAsync();

            return newOrder;
        }


        public async Task UpdateOrderStatus(string userId, TransactionStatus newStatus)
        {
            var order = await _unitOfWork.Orders.FindAsync(
                o => o.UserId == userId && o.TransactionStatus == TransactionStatus.Pending);

            if (order == null)
                throw new Exception("No pending order found for this user.");

            order.TransactionStatus = newStatus;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
        }

    }

}
