using Microsoft.AspNetCore.Identity;
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
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order>Orders{ get; set; }
        public DbSet<Receipt>Receipts { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(config =>
            {
                config.Property(u => u.IsActive)
                .HasDefaultValue(true);

                config.HasOne(u => u.Wishlist)
                 .WithOne()
                 .HasForeignKey<User>(u => u.WishlistId)
                 .OnDelete(DeleteBehavior.NoAction);

                config.HasOne(u => u.Cart)
                 .WithOne()
                 .HasForeignKey<User>(u => u.CartId)
                 .OnDelete(DeleteBehavior.NoAction);

                config.Property(u => u.ImageFileName)
                .HasDefaultValue("/Images/defaultImage.png");
            });   


            modelBuilder.Entity<Order>(config =>
            {
                config.HasOne(o => o.User)
                  .WithMany(u => u.Orders)
                  .HasForeignKey(o => o.UserId)
                  .OnDelete(DeleteBehavior.NoAction);

                config.Property(o => o.Date)
                .HasDefaultValueSql("GETDATE()");

                config.HasOne(o => o.Receipt)
                   .WithOne(r => r.Order)
                   .HasForeignKey<Receipt>(r => r.OrderId)
                   .OnDelete(DeleteBehavior.NoAction);

                config.HasOne(o => o.Cart)
                   .WithMany(c => c.Orders)
                   .HasForeignKey(o => o.CartId)
                   .OnDelete(DeleteBehavior.NoAction);
            });


            modelBuilder.Entity<Product>(config =>
            {
                config.Property(p => p.IsActive)
                .HasDefaultValue(true);

                config.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);
            });
                
           
            modelBuilder.Entity<Review>(config =>
            {
                config.HasOne(r => r.Product)
                       .WithMany(p => p.Reviews)
                       .HasForeignKey(r => r.ProductId)
                       .OnDelete(DeleteBehavior.NoAction);

                config.HasOne(r => r.User)
                      .WithMany(u => u.Reviews)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.NoAction);

                config.Property(r => r.ReviewDate)
                .HasDefaultValueSql("GETDATE()");
            });


            
            modelBuilder.Entity<ProductWishlist>(config =>
            {
                config.HasKey(pw => new { pw.ProductId, pw.WishlistId });

                config.HasOne(pw => pw.Product)
                .WithMany(p => p.ProductWishlists)
                .HasForeignKey(pw => pw.ProductId);

                config.HasOne(pw => pw.Wishlist)
                .WithMany(w => w.ProductWishlists)
                .HasForeignKey(pw => pw.WishlistId);

            });


            modelBuilder.Entity<ProductOrder>(config =>
            {
                config.HasKey(po => new { po.ProductId, po.OrderId });

                config.HasOne(po => po.Product)
                .WithMany(p => p.ProductOrders)
                .HasForeignKey(po => po.ProductId);

                config.HasOne(po => po.Order)
                .WithMany(o => o.ProductOrders)
                .HasForeignKey(po => po.OrderId);
            });

     
            modelBuilder.Entity<ProductCart>(config =>
            {
                config.HasKey(pc => new { pc.ProductId, pc.CartId });

                config.HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCarts)
                .HasForeignKey(pc => pc.ProductId);

                config.HasOne(pc => pc.Cart)
                .WithMany(c => c.ProductCarts)
                .HasForeignKey(pc => pc.CartId);
            });


            modelBuilder.Entity<Category>(config =>
            {
                config.Property(c => c.IsActive)
                .HasDefaultValue(true);

                config.HasIndex(c => c.Name)
                .IsUnique();
            });

            modelBuilder.Entity<Receipt>()
                .Property(r => r.Date)
                .HasDefaultValueSql("GETDATE()");

            base.OnModelCreating(modelBuilder);
        }

    }
}