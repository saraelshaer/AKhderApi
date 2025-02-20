using SmartCartCarbonFootprintApi.Models;


namespace SmartCartCarbonFootprintApi.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Wishlist> Wishlists { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<Cart> Carts { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<Role> Roles { get; }

        Task<int> CompleteAsync();
    }
}

