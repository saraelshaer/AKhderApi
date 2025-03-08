using AKhderApi.Models;


namespace AKhderApi.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Wishlist> Wishlists { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<Cart> Carts { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Discount> Discounts { get;}
        IGenericRepository<ProductWishlist> ProductWishlists { get; }
        IGenericRepository<ProductCart> ProductCarts { get; }
        IGenericRepository<Notification> Notifications { get; }
        IGenericRepository<UserNotification> UserNotifications { get; }
        Task<int> CompleteAsync();
    }
}
