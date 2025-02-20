using SmartCartCarbonFootprintApi.Context;
using SmartCartCarbonFootprintApi.Models;

namespace SmartCartCarbonFootprintApi.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<Product> Products { get; private set; }
        public IGenericRepository<Wishlist> Wishlists { get; private set; }
        public IGenericRepository<Order> Orders { get; private set; }
        public IGenericRepository<Cart> Carts { get; private set; }
        public IGenericRepository<Review> Reviews { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Users = new GenericRepository<User>(_context);
            Products = new GenericRepository<Product>(_context);
            Wishlists = new GenericRepository<Wishlist>(_context);
            Orders = new GenericRepository<Order>(_context);
            Carts = new GenericRepository<Cart>(_context);
            Reviews = new GenericRepository<Review>(_context);
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