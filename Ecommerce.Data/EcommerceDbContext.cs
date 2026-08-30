using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.Core.Entities;

namespace Ecommerce.Data
{
    /// <summary>
    /// EF 6 DbContext mapped (Database-First style) to the pre-existing
    /// LegacyEcommerceDb schema on .\SQLEXPRESS. The connection string is
    /// resolved from the web.config connectionStrings entry named "EcommerceDb".
    /// </summary>
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext()
            : base("name=EcommerceDb")
        {
        }

        public EcommerceDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public virtual DbSet<Product> Products { get { return Set<Product>(); } }

        public virtual DbSet<Category> Categories { get { return Set<Category>(); } }

        public virtual DbSet<ProductImage> ProductImages { get { return Set<ProductImage>(); } }

        public virtual DbSet<ProductVariant> ProductVariants { get { return Set<ProductVariant>(); } }

        public virtual DbSet<CartItem> CartItems { get { return Set<CartItem>(); } }

        public virtual DbSet<Order> Orders { get { return Set<Order>(); } }

        public virtual DbSet<OrderLine> OrderLines { get { return Set<OrderLine>(); } }

        public virtual DbSet<Customer> Customers { get { return Set<Customer>(); } }

        public virtual DbSet<Address> Addresses { get { return Set<Address>(); } }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;

            // ------------------------------------------------------------------
            // Categories (self-referencing tree, lazy loaded)
            // ------------------------------------------------------------------
            modelBuilder.Entity<Category>()
                .ToTable("Categories")
                .HasKey(c => c.Id);

            modelBuilder.Entity<Category>()
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Category>()
                .Property(c => c.Slug)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Categories_Slug") { IsUnique = true }));

            modelBuilder.Entity<Category>()
                .HasOptional(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .WillCascadeOnDelete(false);

            // ------------------------------------------------------------------
            // Products
            // ------------------------------------------------------------------
            modelBuilder.Entity<Product>()
                .ToTable("Products")
                .HasKey(p => p.Id);

            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.ListPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.Sku)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Products_Sku") { IsUnique = true }));

            modelBuilder.Entity<Product>()
                .Property(p => p.Slug)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Products_Slug") { IsUnique = true }));

            modelBuilder.Entity<Product>()
                .Property(p => p.CategoryId)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Products_CategoryId")));

            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<Product>()
                .HasOptional(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .WillCascadeOnDelete(false);

            // ------------------------------------------------------------------
            // Product images / variants
            // ------------------------------------------------------------------
            modelBuilder.Entity<ProductImage>()
                .ToTable("ProductImages")
                .HasKey(i => i.Id);

            modelBuilder.Entity<ProductImage>()
                .Property(i => i.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<ProductImage>()
                .Property(i => i.ProductId)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_ProductImages_ProductId")));

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithRequired(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ProductVariant>()
                .ToTable("ProductVariants")
                .HasKey(v => v.Id);

            modelBuilder.Entity<ProductVariant>()
                .Property(v => v.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<ProductVariant>()
                .Property(v => v.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductVariant>()
                .Property(v => v.ProductId)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_ProductVariants_ProductId")));

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Variants)
                .WithRequired(v => v.Product)
                .HasForeignKey(v => v.ProductId)
                .WillCascadeOnDelete(true);

            // ------------------------------------------------------------------
            // Cart items (session-persisted rows)
            // ------------------------------------------------------------------
            modelBuilder.Entity<CartItem>()
                .ToTable("CartItems")
                .HasKey(c => c.Id);

            modelBuilder.Entity<CartItem>()
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<CartItem>()
                .Property(c => c.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CartItem>()
                .Property(c => c.SessionId)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_CartItems_SessionId")));

            modelBuilder.Entity<CartItem>()
                .Property(c => c.CreatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<CartItem>()
                .Property(c => c.UpdatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<CartItem>()
                .HasRequired(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .WillCascadeOnDelete(false);

            // ------------------------------------------------------------------
            // Customers / addresses
            // ------------------------------------------------------------------
            modelBuilder.Entity<Customer>()
                .ToTable("Customers")
                .HasKey(c => c.Id);

            modelBuilder.Entity<Customer>()
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Customer>()
                .Property(c => c.Email)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Customers_Email") { IsUnique = true }));

            modelBuilder.Entity<Customer>()
                .Property(c => c.CreatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<Address>()
                .ToTable("Addresses")
                .HasKey(a => a.Id);

            modelBuilder.Entity<Address>()
                .Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Address>()
                .Property(a => a.CustomerId)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Addresses_CustomerId")));

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Addresses)
                .WithRequired(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .WillCascadeOnDelete(true);

            // ------------------------------------------------------------------
            // Orders / order lines
            // ------------------------------------------------------------------
            modelBuilder.Entity<Order>()
                .ToTable("Orders")
                .HasKey(o => o.Id);

            modelBuilder.Entity<Order>()
                .Property(o => o.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Order>()
                .Property(o => o.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.Discount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TaxTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.GrandTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderDate)
                .HasColumnType("datetime2");

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderNumber)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Orders_OrderNumber") { IsUnique = true }));

            modelBuilder.Entity<Order>()
                .Property(o => o.CustomerId)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Orders_CustomerId")));

            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasOptional(o => o.Address)
                .WithMany()
                .HasForeignKey(o => o.AddressId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderLine>()
                .ToTable("OrderLines")
                .HasKey(l => l.Id);

            modelBuilder.Entity<OrderLine>()
                .Property(l => l.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<OrderLine>()
                .Property(l => l.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderLine>()
                .Property(l => l.LineTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderLine>()
                .Property(l => l.OrderId)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_OrderLines_OrderId")));

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Lines)
                .WithRequired(l => l.Order)
                .HasForeignKey(l => l.OrderId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<OrderLine>()
                .HasRequired(l => l.Product)
                .WithMany()
                .HasForeignKey(l => l.ProductId)
                .WillCascadeOnDelete(false);

            // ---------------------------------------------------------------
            // Singularize convention off — table names are explicit above.
            // ---------------------------------------------------------------
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}