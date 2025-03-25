using AKhderApi.Context;
using AKhderApi.Models;

namespace AKhderApi.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<Product> Products { get; private set; }
        public IGenericRepository<Wishlist> Wishlists { get; private set; }
        public IGenericRepository<Order> Orders { get; private set; }
        public IGenericRepository<Cart> Carts { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Review> Reviews { get; private set; }
        public IGenericRepository<Discount> Discounts { get; private set; }
        public IGenericRepository<ProductWishlist> ProductWishlists { get; private set; }
        public IGenericRepository<ProductCart> ProductCarts { get; private set; }
        public IGenericRepository<ProductOrder> ProductOrders { get; private set; }

        public IGenericRepository<Notification> Notifications { get; private set; }
        public IGenericRepository<UserNotification> UserNotifications { get; private set; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Users = new GenericRepository<User>(_context);
            Products = new GenericRepository<Product>(_context);
            Wishlists = new GenericRepository<Wishlist>(_context);
            Orders = new GenericRepository<Order>(_context);
            Carts = new GenericRepository<Cart>(_context);
            Reviews = new GenericRepository<Review>(_context);
            Categories = new GenericRepository<Category>(_context);
            Discounts = new GenericRepository<Discount>(_context);
            ProductWishlists = new GenericRepository<ProductWishlist>(_context);
            ProductCarts = new GenericRepository<ProductCart>(_context);
            ProductOrders = new GenericRepository<ProductOrder>(_context);
            Notifications = new GenericRepository<Notification>(_context);
            UserNotifications = new GenericRepository<UserNotification>(_context);
        }


        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}