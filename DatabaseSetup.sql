/* ==========================================================================
   LegacyEcommerceDb - SQL Server Express database setup
   Run this ONCE against a SQL Server Express instance (.\SQLEXPRESS):

       sqlcmd -S .\SQLEXPRESS -i DatabaseSetup.sql

   It creates the database, the limited-privilege application login used by
   the web.config connection string, all tables/indexes/constraints and a
   small catalog of seed data so the store is demo-ready.

   Express notes: no SQL Server Agent on Express — schedule index maintenance
   and backups with Windows Task Scheduler + sqlcmd scripts instead.
   ========================================================================== */

USE [master];
GO

IF DB_ID(N'LegacyEcommerceDb') IS NOT NULL
BEGIN
    ALTER DATABASE [LegacyEcommerceDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [LegacyEcommerceDb];
END
GO

CREATE DATABASE [LegacyEcommerceDb];
GO

/* --------------------------------------------------------------------------
   Application login used by the connection string:
   Server=.\SQLEXPRESS;Database=LegacyEcommerceDb;User Id=legacy_app_user;...

   The password is supplied via a sqlcmd variable so no secret is committed:

       sqlcmd -S .\SQLEXPRESS -v AppUserPassword="YourPass123!" -i DatabaseSetup.sql
   -------------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'legacy_app_user')
BEGIN
    CREATE LOGIN [legacy_app_user] WITH PASSWORD = N'$(AppUserPassword)',
        DEFAULT_DATABASE = [LegacyEcommerceDb], CHECK_POLICY = ON;
END
GO

USE [LegacyEcommerceDb];
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'legacy_app_user')
BEGIN
    CREATE USER [legacy_app_user] FOR LOGIN [legacy_app_user];

    /* data access only — no DDL, no admin */
    EXEC sp_addrolemember N'db_datareader', N'legacy_app_user';
    EXEC sp_addrolemember N'db_datawriter', N'legacy_app_user';
END
GO

/* ==========================================================================
   Tables
   ========================================================================== */

CREATE TABLE dbo.Categories
(
    Id           INT IDENTITY(1,1) NOT NULL,
    Name         NVARCHAR(150)     NOT NULL,
    Slug         NVARCHAR(150)     NOT NULL,
    ParentId     INT               NULL,
    DisplayOrder INT               NOT NULL CONSTRAINT DF_Categories_DisplayOrder DEFAULT (0),
    IsActive     BIT               NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT (1),
    CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Categories_Slug UNIQUE (Slug),
    CONSTRAINT FK_Categories_Parent FOREIGN KEY (ParentId)
        REFERENCES dbo.Categories (Id)
);
GO

CREATE UNIQUE INDEX IX_Categories_Slug ON dbo.Categories (Slug);
CREATE INDEX IX_Categories_ParentId ON dbo.Categories (ParentId);
GO

CREATE TABLE dbo.Products
(
    Id                INT IDENTITY(1,1) NOT NULL,
    Name              NVARCHAR(200)     NOT NULL,
    Slug              NVARCHAR(200)     NOT NULL,
    ShortDescription  NVARCHAR(600)     NULL,
    Description       NVARCHAR(MAX)     NULL,
    Price             DECIMAL(18, 2)    NOT NULL CONSTRAINT DF_Products_Price DEFAULT (0),
    ListPrice         DECIMAL(18, 2)    NULL,
    Sku               NVARCHAR(64)      NOT NULL,
    CategoryId        INT               NULL,
    IsFeatured        BIT               NOT NULL CONSTRAINT DF_Products_IsFeatured DEFAULT (0),
    IsActive          BIT               NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT (1),
    StockQuantity     INT               NOT NULL CONSTRAINT DF_Products_StockQuantity DEFAULT (0),
    ThumbnailUrl      NVARCHAR(600)     NULL,
    CreatedAt         DATETIME2(7)      NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Products_Sku UNIQUE (Sku),
    CONSTRAINT UQ_Products_Slug UNIQUE (Slug),
    CONSTRAINT FK_Products_Category FOREIGN KEY (CategoryId)
        REFERENCES dbo.Categories (Id)
);
GO

CREATE UNIQUE INDEX IX_Products_Sku ON dbo.Products (Sku);
CREATE UNIQUE INDEX IX_Products_Slug ON dbo.Products (Slug);
CREATE INDEX IX_Products_CategoryId ON dbo.Products (CategoryId);
CREATE INDEX IX_Products_ActiveFeatured ON dbo.Products (IsActive, IsFeatured) INCLUDE (Price, CreatedAt);
GO

CREATE TABLE dbo.ProductImages
(
    Id        INT IDENTITY(1,1) NOT NULL,
    ProductId INT               NOT NULL,
    Url       NVARCHAR(600)     NOT NULL,
    AltText   NVARCHAR(200)     NULL,
    SortOrder INT               NOT NULL CONSTRAINT DF_ProductImages_SortOrder DEFAULT (0),
    IsPrimary BIT               NOT NULL CONSTRAINT DF_ProductImages_IsPrimary DEFAULT (0),
    CONSTRAINT PK_ProductImages PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_ProductImages_Product FOREIGN KEY (ProductId)
        REFERENCES dbo.Products (Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_ProductImages_ProductId ON dbo.ProductImages (ProductId);
GO

CREATE TABLE dbo.ProductVariants
(
    Id            INT IDENTITY(1,1) NOT NULL,
    ProductId     INT               NOT NULL,
    Name          NVARCHAR(100)     NOT NULL,
    Sku           NVARCHAR(64)      NOT NULL,
    Price         DECIMAL(18, 2)    NOT NULL,
    StockQuantity INT               NOT NULL CONSTRAINT DF_ProductVariants_StockQuantity DEFAULT (0),
    IsActive      BIT               NOT NULL CONSTRAINT DF_ProductVariants_IsActive DEFAULT (1),
    CONSTRAINT PK_ProductVariants PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_ProductVariants_Sku UNIQUE (Sku),
    CONSTRAINT FK_ProductVariants_Product FOREIGN KEY (ProductId)
        REFERENCES dbo.Products (Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_ProductVariants_ProductId ON dbo.ProductVariants (ProductId);
GO

CREATE TABLE dbo.CartItems
(
    Id          INT IDENTITY(1,1) NOT NULL,
    SessionId   UNIQUEIDENTIFIER  NOT NULL,
    ProductId   INT               NOT NULL,
    ProductName NVARCHAR(200)     NOT NULL,
    Sku         NVARCHAR(64)      NOT NULL,
    UnitPrice   DECIMAL(18, 2)    NOT NULL,
    Quantity    INT               NOT NULL CONSTRAINT DF_CartItems_Quantity DEFAULT (1),
    CreatedAt   DATETIME2(7)      NOT NULL CONSTRAINT DF_CartItems_CreatedAt DEFAULT (SYSDATETIME()),
    UpdatedAt   DATETIME2(7)      NULL,
    CONSTRAINT PK_CartItems PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_CartItems_Product FOREIGN KEY (ProductId)
        REFERENCES dbo.Products (Id)
);
GO

CREATE INDEX IX_CartItems_SessionId ON dbo.CartItems (SessionId);
GO

CREATE TABLE dbo.Customers
(
    Id            INT IDENTITY(1,1) NOT NULL,
    Email         NVARCHAR(200)     NOT NULL,
    PasswordHash  NVARCHAR(500)     NOT NULL,
    PasswordSalt  NVARCHAR(100)     NOT NULL,
    FirstName     NVARCHAR(100)     NOT NULL,
    LastName      NVARCHAR(100)     NOT NULL,
    Phone         NVARCHAR(40)      NULL,
    IsActive      BIT               NOT NULL CONSTRAINT DF_Customers_IsActive DEFAULT (1),
    CreatedAt     DATETIME2(7)      NOT NULL CONSTRAINT DF_Customers_CreatedAt DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Customers PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Customers_Email UNIQUE (Email)
);
GO

CREATE UNIQUE INDEX IX_Customers_Email ON dbo.Customers (Email);
GO

CREATE TABLE dbo.Addresses
(
    Id           INT IDENTITY(1,1) NOT NULL,
    CustomerId   INT               NOT NULL,
    FirstName    NVARCHAR(100)     NOT NULL,
    LastName     NVARCHAR(100)     NOT NULL,
    AddressLine1 NVARCHAR(200)     NOT NULL,
    AddressLine2 NVARCHAR(200)     NULL,
    City         NVARCHAR(100)     NOT NULL,
    State        NVARCHAR(100)     NULL,
    PostalCode   NVARCHAR(20)      NOT NULL,
    Country      NVARCHAR(100)     NULL,
    Phone        NVARCHAR(40)      NULL,
    IsDefault    BIT               NOT NULL CONSTRAINT DF_Addresses_IsDefault DEFAULT (0),
    CONSTRAINT PK_Addresses PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Addresses_Customer FOREIGN KEY (CustomerId)
        REFERENCES dbo.Customers (Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_Addresses_CustomerId ON dbo.Addresses (CustomerId);
GO

CREATE TABLE dbo.Orders
(
    Id             INT IDENTITY(1,1) NOT NULL,
    CustomerId     INT               NOT NULL,
    AddressId      INT               NULL,
    OrderNumber    NVARCHAR(32)      NOT NULL,
    OrderDate      DATETIME2(7)      NOT NULL CONSTRAINT DF_Orders_OrderDate DEFAULT (SYSDATETIME()),
    Status         NVARCHAR(20)      NOT NULL CONSTRAINT DF_Orders_Status DEFAULT (N'Pending'),
    CouponCode     NVARCHAR(40)      NULL,
    Subtotal       DECIMAL(18, 2)    NOT NULL,
    Discount       DECIMAL(18, 2)    NOT NULL CONSTRAINT DF_Orders_Discount DEFAULT (0),
    ShippingTotal  DECIMAL(18, 2)    NOT NULL,
    TaxTotal       DECIMAL(18, 2)    NOT NULL,
    GrandTotal     DECIMAL(18, 2)    NOT NULL,
    PaymentMethod  NVARCHAR(40)      NOT NULL,
    TransactionId  NVARCHAR(100)     NULL,
    ShippingMethod NVARCHAR(40)      NULL,
    Email          NVARCHAR(200)     NOT NULL,
    ShipToName     NVARCHAR(200)     NULL,
    AddressLine1   NVARCHAR(200)     NULL,
    AddressLine2   NVARCHAR(200)     NULL,
    City           NVARCHAR(100)     NULL,
    State          NVARCHAR(100)     NULL,
    PostalCode     NVARCHAR(20)      NULL,
    Country        NVARCHAR(100)     NULL,
    Phone          NVARCHAR(40)      NULL,
    CONSTRAINT PK_Orders PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Orders_OrderNumber UNIQUE (OrderNumber),
    CONSTRAINT FK_Orders_Customer FOREIGN KEY (CustomerId)
        REFERENCES dbo.Customers (Id),
    CONSTRAINT FK_Orders_Address FOREIGN KEY (AddressId)
        REFERENCES dbo.Addresses (Id)
);
GO

CREATE UNIQUE INDEX IX_Orders_OrderNumber ON dbo.Orders (OrderNumber);
CREATE INDEX IX_Orders_CustomerId ON dbo.Orders (CustomerId);
GO

CREATE TABLE dbo.OrderLines
(
    Id          INT IDENTITY(1,1) NOT NULL,
    OrderId     INT               NOT NULL,
    ProductId   INT               NULL,
    ProductName NVARCHAR(200)     NOT NULL,
    Sku         NVARCHAR(64)      NOT NULL,
    UnitPrice   DECIMAL(18, 2)    NOT NULL,
    Quantity    INT               NOT NULL,
    LineTotal   DECIMAL(18, 2)    NOT NULL,
    CONSTRAINT PK_OrderLines PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_OrderLines_Order FOREIGN KEY (OrderId)
        REFERENCES dbo.Orders (Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderLines_Product FOREIGN KEY (ProductId)
        REFERENCES dbo.Products (Id)
);
GO

CREATE INDEX IX_OrderLines_OrderId ON dbo.OrderLines (OrderId);
GO

/* ==========================================================================
   Seed data
   ========================================================================== */

INSERT INTO dbo.Categories (Name, Slug, ParentId, DisplayOrder, IsActive) VALUES
    (N'Electronics', N'electronics', NULL, 1, 1),
    (N'Clothing',    N'clothing',    NULL, 2, 1),
    (N'Home & Garden', N'home-garden', NULL, 3, 1),
    (N'Computers',   N'computers',   1, 1, 1),
    (N'Phones',      N'phones',      1, 2, 1),
    (N'Men',         N'men',         2, 1, 1),
    (N'Women',       N'women',       2, 2, 1),
    (N'Kitchen',     N'kitchen',     3, 1, 1);
GO

INSERT INTO dbo.Products (Name, Slug, ShortDescription, Description, Price, ListPrice, Sku, CategoryId, IsFeatured, StockQuantity, ThumbnailUrl) VALUES
    (N'Wireless Ergonomic Mouse', N'wireless-ergonomic-mouse', N'Smooth wireless ergonomic mouse for all-day comfort.',
     N'<p>A classic wireless optical mouse with an ergonomic shape, 2.4 GHz USB receiver and 12-month battery life.</p><ul><li>Unifying receiver</li><li>1800 DPI</li><li>Silent clicks</li></ul>',
     29.99, 39.99, N'ELC-1001', 4, 1, 120, N'/Content/Images/p1.svg'),
    (N'15.6" Laptop Backpack', N'laptop-backpack', N'Water-resistant backpack with padded 15.6" laptop sleeve.',
     N'<p>Durable daily-carry backpack with a padded 15.6-inch laptop compartment, USB charging port and luggage strap.</p>',
     49.95, NULL, N'ELC-1002', 4, 1, 60, N'/Content/Images/p2.svg'),
    (N'Noise-Cancelling Headphones', N'noise-cancelling-headphones', N'Over-ear headphones with active noise cancelling.',
     N'<p>Over-ear Bluetooth headphones delivering rich sound and deep bass with active noise cancellation and 30-hour playtime.</p>',
     149.00, 199.00, N'ELC-1003', 1, 1, 75, N'/Content/Images/p3.svg'),
    (N'Smart Watch - 42mm', N'smart-watch', N'GPS smart watch with heart-rate monitoring.',
     N'<p>Track workouts, sleep and calls from your wrist. Water resistant to 50m.</p>',
     199.99, NULL, N'ELC-2001', 5, 1, 40, N'/Content/Images/p4.svg'),
    (N'Classic Cotton Hoodie', N'classic-cotton-hoodie', N'Soft essentials hoodie — brushed cotton fleece.',
     N'<p>A wardrobe staple cut from heavyweight brushed-back cotton fleece. Available in sizes S-XXL.</p>',
     39.99, 49.99, N'CLO-3001', 6, 1, 200, N'/Content/Images/p5.svg'),
    (N'Slim Fit Chino Pants', N'slim-fit-chino-pants', N'Tailored slim-fit chinos in a 4-way stretch fabric.',
     N'<p>Versatile chinos with a modern slim fit, moisture-wicking stretch fabric and permanent crease.</p>',
     54.50, NULL, N'CLO-3002', 6, 1, 90, N'/Content/Images/p6.svg'),
    (N'Stainless Steel Water Bottle', N'stainless-steel-water-bottle', N'Double-wall insulated bottle keeps drinks cold 24h, hot 12h.',
     N'<p>Vacuum-insulated 750ml bottle. Sweat-free exterior, leak-proof lid, BPA-free.</p>',
     24.99, 32.99, N'HOM-4001', 8, 1, 150, N'/Content/Images/p7.svg'),
    (N'Ceramic Non-Stick Cookware Set', N'ceramic-cookware-set', N'12-piece ceramic non-stick cookware set, oven-safe to 400F.',
     N'<p>Lead- and cadmium-free ceramic coating, induction compatible, dishwasher safe. Includes pots, pans and lids.</p>',
     189.00, NULL, N'HOM-4002', 8, 1, 25, N'/Content/Images/p8.svg');
GO

/* Primary images for each product */
INSERT INTO dbo.ProductImages (ProductId, Url, AltText, SortOrder, IsPrimary) VALUES
    (1, N'/Content/Images/p1.svg', N'Wireless Ergonomic Mouse', 0, 1),
    (2, N'/Content/Images/p2.svg', N'Laptop Backpack', 0, 1),
    (3, N'/Content/Images/p3.svg', N'Noise-Cancelling Headphones', 0, 1),
    (4, N'/Content/Images/p4.svg', N'Smart Watch', 0, 1),
    (5, N'/Content/Images/p5.svg', N'Classic Cotton Hoodie', 0, 1),
    (6, N'/Content/Images/p6.svg', N'Slim Fit Chino Pants', 0, 1),
    (7, N'/Content/Images/p7.svg', N'Stainless Steel Water Bottle', 0, 1),
    (8, N'/Content/Images/p8.svg', N'Ceramic Cookware Set', 0, 1);
GO

/* Variants for a couple of products */
INSERT INTO dbo.ProductVariants (ProductId, Name, Sku, Price, StockQuantity, IsActive) VALUES
    (5, N'Size M', N'CLO-3001-M', 39.99, 50, 1),
    (5, N'Size L', N'CLO-3001-L', 39.99, 80, 1),
    (5, N'Size XL', N'CLO-3001-XL', 39.99, 40, 1),
    (6, N'Size 30', N'CLO-3002-30', 54.50, 30, 1),
    (6, N'Size 32', N'CLO-3002-32', 54.50, 30, 1),
    (6, N'Size 34', N'CLO-3002-34', 54.50, 30, 1);
GO

/*
   Demo customer:
      Email:    demo@legacy.store
      Password: Password123!
   PBKDF2 (Rfc2898DeriveBytes, 10000 iterations, 16-byte salt, 32-byte hash)
   matching Ecommerce.Services.Security.PasswordHasher.
*/
INSERT INTO dbo.Customers (Email, PasswordHash, PasswordSalt, FirstName, LastName, Phone, IsActive) VALUES
    (N'demo@legacy.store',
     N'fYOnBC7Rx0vvW9wmrxugGb8F/cC52iaRjqjtXf4DqMQ=',
     N'+1vxs2cMUjc2Pfd3w+4f4g==',
     N'Demo', N'User', N'555-0100', 1);
GO

INSERT INTO dbo.Addresses (CustomerId, FirstName, LastName, AddressLine1, AddressLine2, City, State, PostalCode, Country, Phone, IsDefault) VALUES
    (1, N'Demo', N'User', N'1 Main Street', NULL, N'Redmond', N'WA', N'98052', N'US', N'555-0100', 1);
GO

/* ==========================================================================
   Maintenance notes (no SQL Agent on Express — run from Task Scheduler):
   Weekly index rebuild:
     ALTER INDEX ALL ON dbo.Orders REBUILD;   -- repeat for other tables
   Backup:
     BACKUP DATABASE [LegacyEcommerceDb] TO DISK = N'C:\Backups\LegacyEcommerceDb.bak';
   ========================================================================== */