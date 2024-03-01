using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecomm.Data
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options) { }

        #region DbSet
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<CustomerData> Customers { get; set; }
        public DbSet<ProductData> Products { get; set; }
        public DbSet<CategoryData> Categories { get; set; }
        public DbSet<OrderData> Orders { get; set; }
        public DbSet<OrderDetailsData> OrderDetails { get; set; }
        #endregion 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Write Fluent API configurations here

            //Property Configurations

            modelBuilder.Entity<ProductData>(e =>
            {
                e.ToTable("Products");
                e.HasKey(p => p.ProductID);


            });

            modelBuilder.Entity<CategoryData>(e =>
            {
                e.ToTable("Categories");
                e.HasKey(c => c.CategoryID);

                e.HasMany(c => c.Product)  // CategoryData has many Products
                .WithOne(p => p.Category)  // ProductData has one Category
                .HasForeignKey(p => p.CategoryID)
                .HasConstraintName("FK_Product_Category");
            });

            modelBuilder.Entity<OrderData>(e =>
            {
                e.ToTable("Orders");
                e.HasKey(o => o.OrderID);
                e.Property(o => o.OrderDate).HasDefaultValueSql("getutcdate()");
            });

            modelBuilder.Entity<OrderDetailsData>(e =>
            {
                e.ToTable("OrderDetails");
                e.HasKey(od => od.OrderID);

                e.HasOne(e => e.Order)
                .WithMany(e => e.OrderDetail)
                .HasForeignKey(e => e.OrderID)
                .HasConstraintName("FK_OrderDetails_Order");


                e.HasOne(e => e.Product)
                .WithMany(e => e.OrderDetail)
                .HasForeignKey(e => e.ProductID)
                .HasConstraintName("FK_OrderDetails_Product");
            });

            modelBuilder.Entity<CustomerData>(e =>
            {
                e.ToTable("Customer");
                e.HasKey(c => c.CustomerID);

                e.HasIndex(c => c.Email).IsUnique();
                e.Property(c => c.Name).IsRequired().HasMaxLength(150);
            });
        }
    }
}
