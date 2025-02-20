using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartCartCarbonFootprintApi.Models;
using System;

namespace SmartCartCarbonFootprintApi.Context
{
    public class AppDbContext :IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        //public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order>Orders{ get; set; }
        public DbSet<Receipt>Receipts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
    .HasOne(u => u.Wishlist)
    .WithOne(w => w.User)
    .HasForeignKey<User>(u => u.WishlistId)
    .OnDelete(DeleteBehavior.NoAction);

            //-------------------------
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            //-------------------------
            modelBuilder.Entity<User>()
    .HasOne(u => u.Cart)
    .WithOne()
    .HasForeignKey<User>(u => u.CartId)
    .OnDelete(DeleteBehavior.NoAction);


            //-------------------------

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            //-------------------------
            modelBuilder.Entity<Product>()
    .HasOne(p => p.Category)
    .WithMany(c => c.Products)
    .HasForeignKey(p => p.CategoryId)
    .OnDelete(DeleteBehavior.NoAction);
            //-------------------------
            modelBuilder.Entity<Review>()
    .HasOne(r => r.Product)
    .WithMany(p => p.Reviews)
    .HasForeignKey(r => r.ProductId)
    .OnDelete(DeleteBehavior.NoAction);
            //-------------------------
            modelBuilder.Entity<ProductWishlist>()
             .HasKey(pw => new { pw.ProductId, pw.WishlistId }); 

            modelBuilder.Entity<ProductWishlist>()
                .HasOne(pw => pw.Product)
                .WithMany(p => p.ProductWishlists)
                .HasForeignKey(pw => pw.ProductId);

            modelBuilder.Entity<ProductWishlist>()
                .HasOne(pw => pw.Wishlist)
                .WithMany(w => w.ProductWishlists)
                .HasForeignKey(pw => pw.WishlistId);

            //-------------------------
            modelBuilder.Entity<ProductOrder>()
    .HasKey(po => new { po.ProductId, po.OrderId });

            modelBuilder.Entity<ProductOrder>()
                .HasOne(po => po.Product)
                .WithMany(p => p.ProductOrders)
                .HasForeignKey(po => po.ProductId);

            modelBuilder.Entity<ProductOrder>()
                .HasOne(po => po.Order)
                .WithMany(o => o.ProductOrders)
                .HasForeignKey(po => po.OrderId);

            //-------------------------
            modelBuilder.Entity<ProductCart>()
    .HasKey(pc => new { pc.ProductId, pc.CartId });

            modelBuilder.Entity<ProductCart>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCarts)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductCart>()
                .HasOne(pc => pc.Cart)
                .WithMany(c => c.ProductCarts)
                .HasForeignKey(pc => pc.CartId);

            //-------------------------
            modelBuilder.Entity<Order>()
    .HasOne(o => o.Receipt)
    .WithOne(r => r.Order)
    .HasForeignKey<Receipt>(r => r.OrderId)
    .OnDelete(DeleteBehavior.NoAction);

            //-------------------------
            modelBuilder.Entity<Order>()
    .HasOne(o => o.Cart)
    .WithMany(c => c.Orders)
    .HasForeignKey(o => o.CartId)
    .OnDelete(DeleteBehavior.NoAction);






        }

    }
}
