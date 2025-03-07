using AKhderApi.Models;
using AKhderApi.Repositories;

namespace SmartCartCarbonFootprintApi.Services
{
    public class OrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateOrder(string userId)
        {
            var cart = (await _unitOfWork.Carts.GetAllAsync(c => c.User.Id == userId, new[] { "ProductCarts.Product" }))
                       .FirstOrDefault();

            if (cart == null || !cart.ProductCarts.Any())
                throw new Exception("Cart is empty");

            var order = new Order
            {
                UserId = userId,
                CartId = cart.Id,
                TotalPrice = cart.ProductCarts.Sum(cp => cp.Product.Price),
                CreatedAt = DateTime.UtcNow,
                ProductOrders = cart.ProductCarts.Select(cp => new ProductOrder
                {
                    ProductId = cp.ProductId,
                }).ToList()
            };

            await _unitOfWork.Orders.AddAsync(order);
            cart.ProductCarts.Clear(); // حذف المنتجات من السلة بعد إنشاء الطلب
            await _unitOfWork.CompleteAsync();
        }
    }

}
