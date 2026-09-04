using Microsoft.EntityFrameworkCore;
using Ecommerce.Core.Entities;

namespace Ecommerce.Data
{
    /// <summary>
    /// EF Core 10 DbContext mapped to the pre-existing LegacyEcommerceDb schema
    /// on .\SQLEXPRESS. The connection string is resolved from the "EcommerceDb"
    /// connection string registered in the hosting application.
    /// </summary>
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext()
        {
        }

        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<ProductVariant> ProductVariants { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderLine> OrderLines { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ------------------------------------------------------------------
            // Categories (self-referencing tree)
            // ------------------------------------------------------------------
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
                entity.Property(c => c.Slug).IsRequired().HasMaxLength(150);
                entity.HasIndex(c => c.Slug).IsUnique();
                entity.Property(c => c.Name).IsRequired().HasMaxLength(150);
                entity.Property(c => c.DisplayOrder).HasDefaultValue(0);
                entity.Property(c => c.IsActive).HasDefaultValue(true);

                entity.HasOne(c => c.Parent)
                    .WithMany(c => c.Children)
                    .HasForeignKey(c => c.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ------------------------------------------------------------------
            // Products
            // ------------------------------------------------------------------
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Slug).IsRequired().HasMaxLength(200);
                entity.Property(p => p.ShortDescription).HasMaxLength(600);
                entity.Property(p => p.Price).HasPrecision(18, 2);
                entity.Property(p => p.ListPrice).HasPrecision(18, 2);
                entity.Property(p => p.Sku).IsRequired().HasMaxLength(64);
                entity.Property(p => p.ThumbnailUrl).HasMaxLength(600);
                entity.Property(p => p.CreatedAt).HasColumnType("datetime2");

                entity.HasIndex(p => p.Sku).IsUnique();
                entity.HasIndex(p => p.Slug).IsUnique();
                entity.HasIndex(p => p.CategoryId);
                entity.HasIndex(p => new { p.IsActive, p.IsFeatured });

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ------------------------------------------------------------------
            // Product images (cascade delete with product)
            // ------------------------------------------------------------------
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImages");
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Id).ValueGeneratedOnAdd();
                entity.Property(i => i.Url).IsRequired().HasMaxLength(600);
                entity.Property(i => i.AltText).HasMaxLength(200);

                entity.HasIndex(i => i.ProductId);

                entity.HasOne(i => i.Product)
                    .WithMany(p => p.Images)
                    .HasForeignKey(i => i.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ------------------------------------------------------------------
            // Product variants (cascade delete with product)
            // ------------------------------------------------------------------
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.ToTable("ProductVariants");
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Id).ValueGeneratedOnAdd();
                entity.Property(v => v.Name).IsRequired().HasMaxLength(100);
                entity.Property(v => v.Sku).IsRequired().HasMaxLength(64);
                entity.Property(v => v.Price).HasPrecision(18, 2);

                entity.HasIndex(v => v.Sku).IsUnique();
                entity.HasIndex(v => v.ProductId);

                entity.HasOne(v => v.Product)
                    .WithMany(p => p.Variants)
                    .HasForeignKey(v => v.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ------------------------------------------------------------------
            // Cart items (session-persisted rows)
            // ------------------------------------------------------------------
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("CartItems");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
                entity.Property(c => c.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Sku).IsRequired().HasMaxLength(64);
                entity.Property(c => c.UnitPrice).HasPrecision(18, 2);
                entity.Property(c => c.CreatedAt).HasColumnType("datetime2");
                entity.Property(c => c.UpdatedAt).HasColumnType("datetime2");

                entity.HasIndex(c => c.SessionId);

                entity.HasOne(c => c.Product)
                    .WithMany()
                    .HasForeignKey(c => c.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ------------------------------------------------------------------
            // Customers / addresses (cascade addresses)
            // ------------------------------------------------------------------
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
                entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
                entity.Property(c => c.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(c => c.PasswordSalt).IsRequired().HasMaxLength(100);
                entity.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(c => c.LastName).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Phone).HasMaxLength(40);
                entity.Property(c => c.CreatedAt).HasColumnType("datetime2");

                entity.HasIndex(c => c.Email).IsUnique();
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Addresses");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).ValueGeneratedOnAdd();
                entity.Property(a => a.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(a => a.LastName).IsRequired().HasMaxLength(100);
                entity.Property(a => a.AddressLine1).IsRequired().HasMaxLength(200);
                entity.Property(a => a.AddressLine2).HasMaxLength(200);
                entity.Property(a => a.City).IsRequired().HasMaxLength(100);
                entity.Property(a => a.State).HasMaxLength(100);
                entity.Property(a => a.PostalCode).IsRequired().HasMaxLength(20);
                entity.Property(a => a.Country).HasMaxLength(100);
                entity.Property(a => a.Phone).HasMaxLength(40);

                entity.HasIndex(a => a.CustomerId);

                entity.HasOne(a => a.Customer)
                    .WithMany(c => c.Addresses)
                    .HasForeignKey(a => a.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ------------------------------------------------------------------
            // Orders / order lines
            // ------------------------------------------------------------------
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).ValueGeneratedOnAdd();
                entity.Property(o => o.OrderNumber).IsRequired().HasMaxLength(32);
                entity.Property(o => o.OrderDate).HasColumnType("datetime2");
                entity.Property(o => o.Status).IsRequired().HasMaxLength(20);
                entity.Property(o => o.CouponCode).HasMaxLength(40);
                entity.Property(o => o.Subtotal).HasPrecision(18, 2);
                entity.Property(o => o.Discount).HasPrecision(18, 2);
                entity.Property(o => o.ShippingTotal).HasPrecision(18, 2);
                entity.Property(o => o.TaxTotal).HasPrecision(18, 2);
                entity.Property(o => o.GrandTotal).HasPrecision(18, 2);
                entity.Property(o => o.PaymentMethod).IsRequired().HasMaxLength(40);
                entity.Property(o => o.TransactionId).HasMaxLength(100);
                entity.Property(o => o.ShippingMethod).HasMaxLength(40);
                entity.Property(o => o.Email).IsRequired().HasMaxLength(200);
                entity.Property(o => o.ShipToName).HasMaxLength(200);
                entity.Property(o => o.AddressLine1).HasMaxLength(200);
                entity.Property(o => o.AddressLine2).HasMaxLength(200);
                entity.Property(o => o.City).HasMaxLength(100);
                entity.Property(o => o.State).HasMaxLength(100);
                entity.Property(o => o.PostalCode).HasMaxLength(20);
                entity.Property(o => o.Country).HasMaxLength(100);
                entity.Property(o => o.Phone).HasMaxLength(40);

                entity.HasIndex(o => o.OrderNumber).IsUnique();
                entity.HasIndex(o => o.CustomerId);

                entity.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Address)
                    .WithMany()
                    .HasForeignKey(o => o.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderLine>(entity =>
            {
                entity.ToTable("OrderLines");
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Id).ValueGeneratedOnAdd();
                entity.Property(l => l.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(l => l.Sku).IsRequired().HasMaxLength(64);
                entity.Property(l => l.UnitPrice).HasPrecision(18, 2);
                entity.Property(l => l.LineTotal).HasPrecision(18, 2);

                entity.HasIndex(l => l.OrderId);

                entity.HasOne(l => l.Order)
                    .WithMany(o => o.Lines)
                    .HasForeignKey(l => l.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.Product)
                    .WithMany()
                    .HasForeignKey(l => l.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
