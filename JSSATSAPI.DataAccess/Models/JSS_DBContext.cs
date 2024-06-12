using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class JSS_DBContext : DbContext
    {
        public JSS_DBContext()
        {
        }

        public JSS_DBContext(DbContextOptions<JSS_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<CategoryType> CategoryTypes { get; set; } = null!;
        public virtual DbSet<Counter> Counters { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Diamond> Diamonds { get; set; } = null!;
        public virtual DbSet<DiamondPrice> DiamondPrices { get; set; } = null!;
        public virtual DbSet<Material> Materials { get; set; } = null!;
        public virtual DbSet<MaterialPrice> MaterialPrices { get; set; } = null!;
        public virtual DbSet<MaterialType> MaterialTypes { get; set; } = null!;
        public virtual DbSet<Membership> Memberships { get; set; } = null!;
        public virtual DbSet<OrderBuyBack> OrderBuyBacks { get; set; } = null!;
        public virtual DbSet<OrderBuyBackDetail> OrderBuyBackDetails { get; set; } = null!;
        public virtual DbSet<OrderSell> OrderSells { get; set; } = null!;
        public virtual DbSet<OrderSellDetail> OrderSellDetails { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<PaymentType> PaymentTypes { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductDiamond> ProductDiamonds { get; set; } = null!;
        public virtual DbSet<ProductMaterial> ProductMaterials { get; set; } = null!;
        public virtual DbSet<WarrantyTicket> WarrantyTickets { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=WITHNHAN\\WTIHNHAN;Initial Catalog=JSS_DB;Persist Security Info=False;User ID=sa;Password=123456;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Revenue).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.Role).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName).HasMaxLength(255);

                entity.Property(e => e.CategoryTypeId).HasColumnName("CategoryTypeID");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.CategoryType)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.CategoryTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Category_CategoryType");
            });

            modelBuilder.Entity<CategoryType>(entity =>
            {
                entity.ToTable("CategoryType");

                entity.Property(e => e.CategoryTypeId).HasColumnName("CategoryTypeID");

                entity.Property(e => e.CategoryTypeName).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.ToTable("Counter");

                entity.Property(e => e.CounterId).HasColumnName("CounterID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CounterName).HasMaxLength(255);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Counters)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Counter_Account");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Tier)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.TierId)
                    .HasConstraintName("FK_Customer_Membership");
            });

            modelBuilder.Entity<Diamond>(entity =>
            {
                entity.HasKey(e => e.DiamondCode)
                    .HasName("PK__Diamond__1A0326039E557E90");

                entity.ToTable("Diamond");

                entity.Property(e => e.DiamondCode).HasMaxLength(255);

                entity.Property(e => e.CaratWeight).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Clarity).HasMaxLength(50);

                entity.Property(e => e.Color).HasMaxLength(50);

                entity.Property(e => e.Cut).HasMaxLength(50);

                entity.Property(e => e.DiamondName).HasMaxLength(255);

                entity.Property(e => e.Origin).HasMaxLength(255);

                entity.Property(e => e.Polish).HasMaxLength(50);

                entity.Property(e => e.Proportions).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Symmetry).HasMaxLength(50);
            });

            modelBuilder.Entity<DiamondPrice>(entity =>
            {
                entity.ToTable("DiamondPrice");

                entity.Property(e => e.DiamondPriceId).HasColumnName("DiamondPriceID");

                entity.Property(e => e.BuyPrice).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.CaratWeight).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Clarity).HasMaxLength(50);

                entity.Property(e => e.Color).HasMaxLength(50);

                entity.Property(e => e.Cut).HasMaxLength(50);

                entity.Property(e => e.EffDate).HasColumnType("datetime");

                entity.Property(e => e.Origin).HasMaxLength(255);

                entity.Property(e => e.SellPrice).HasColumnType("decimal(19, 4)");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.ToTable("Material");

                entity.Property(e => e.MaterialName).HasMaxLength(255);

                entity.HasOne(d => d.MaterialType)
                    .WithMany(p => p.Materials)
                    .HasForeignKey(d => d.MaterialTypeId)
                    .HasConstraintName("FK_Material_MaterialType");
            });

            modelBuilder.Entity<MaterialPrice>(entity =>
            {
                entity.ToTable("MaterialPrice");

                entity.Property(e => e.BuyPrice).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.EffDate).HasColumnType("datetime");

                entity.Property(e => e.SellPrice).HasColumnType("decimal(19, 4)");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.MaterialPrices)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MaterialPrice_Material");
            });

            modelBuilder.Entity<MaterialType>(entity =>
            {
                entity.ToTable("MaterialType");

                entity.Property(e => e.MaterialTypeId).ValueGeneratedNever();

                entity.Property(e => e.MaterialTypeName).HasMaxLength(50);
            });

            modelBuilder.Entity<Membership>(entity =>
            {
                entity.HasKey(e => e.TierId)
                    .HasName("PK__Membersh__362F561D2E5AD5CE");

                entity.ToTable("Membership");

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.TierName).HasMaxLength(255);
            });

            modelBuilder.Entity<OrderBuyBack>(entity =>
            {
                entity.ToTable("OrderBuyBack");

                entity.Property(e => e.OrderBuyBackId).HasColumnName("OrderBuyBackID");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.DateBuyBack).HasColumnType("datetime");

                entity.Property(e => e.FinalAmount).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(19, 4)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.OrderBuyBacks)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_OrderBuyBack_Customer");
            });

            modelBuilder.Entity<OrderBuyBackDetail>(entity =>
            {
                entity.ToTable("OrderBuyBackDetail");

                entity.Property(e => e.OrderBuyBackDetailId).HasColumnName("OrderBuyBackDetailID");

                entity.Property(e => e.BuyBackProductName).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.CaratWeight).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Clarity).HasMaxLength(50);

                entity.Property(e => e.Color).HasMaxLength(50);

                entity.Property(e => e.Cut).HasMaxLength(50);

                entity.Property(e => e.OrderBuyBackId).HasColumnName("OrderBuyBackID");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.Origin).HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("ProductID");

                entity.Property(e => e.Weight).HasColumnType("decimal(10, 4)");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.OrderBuyBackDetails)
                    .HasForeignKey(d => d.MaterialId)
                    .HasConstraintName("FK_OrderBuyBackDetail_Material");

                entity.HasOne(d => d.OrderBuyBack)
                    .WithMany(p => p.OrderBuyBackDetails)
                    .HasForeignKey(d => d.OrderBuyBackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderBuyBackDetail_OrderBuyBack");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderBuyBackDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_OrderBuyBackDetail_Product");
            });

            modelBuilder.Entity<OrderSell>(entity =>
            {
                entity.ToTable("OrderSell");

                entity.Property(e => e.OrderSellId).HasColumnName("OrderSellID");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.FinalAmount).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.InvidualPromotionDiscount).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.MemberShipDiscount).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.PromotionReason).HasMaxLength(255);

                entity.Property(e => e.SellerId).HasColumnName("SellerID");

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(19, 4)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.OrderSells)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_OrderSell_Customer");

                entity.HasOne(d => d.Seller)
                    .WithMany(p => p.OrderSells)
                    .HasForeignKey(d => d.SellerId)
                    .HasConstraintName("FK_OrderSell_Account");
            });

            modelBuilder.Entity<OrderSellDetail>(entity =>
            {
                entity.ToTable("OrderSellDetail");

                entity.Property(e => e.OrderSellDetailId).HasColumnName("OrderSellDetailID");

                entity.Property(e => e.OrderSellId).HasColumnName("OrderSellID");

                entity.Property(e => e.Price).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("ProductID");

                entity.HasOne(d => d.OrderSell)
                    .WithMany(p => p.OrderSellDetails)
                    .HasForeignKey(d => d.OrderSellId)
                    .HasConstraintName("FK_OrderSellDetail_OrderSell");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderSellDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderSellDetail_Product");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.Amount).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.OrderSellId).HasColumnName("OrderSellID");

                entity.HasOne(d => d.OrderBuyBack)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderBuyBackId)
                    .HasConstraintName("FK_Payment_OrderBuyBack");

                entity.HasOne(d => d.OrderSell)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderSellId)
                    .HasConstraintName("FK_Payment_Order");

                entity.HasOne(d => d.PaymentType)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.PaymentTypeId)
                    .HasConstraintName("FK_Payment_PaymentType");
            });

            modelBuilder.Entity<PaymentType>(entity =>
            {
                entity.ToTable("PaymentType");

                entity.Property(e => e.PaymentTypeName).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(100);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.DiamondCost).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.MaterialCost).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.PriceRate).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.ProductName).HasMaxLength(255);

                entity.Property(e => e.ProductPrice).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.ProductionCost).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.Size)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Product_Category");

                entity.HasOne(d => d.Counter)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CounterId)
                    .HasConstraintName("FK_Product_Counter");
            });

            modelBuilder.Entity<ProductDiamond>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ProductDiamond");

                entity.HasIndex(e => e.DiamondCode, "UQ__ProductD__1A0326023191E949")
                    .IsUnique();

                entity.HasIndex(e => e.DiamondCode, "UQ__ProductD__1A0326028DF3D0AA")
                    .IsUnique();

                entity.Property(e => e.DiamondCode).HasMaxLength(255);

                entity.Property(e => e.ProductId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.DiamondCodeNavigation)
                    .WithOne()
                    .HasForeignKey<ProductDiamond>(d => d.DiamondCode)
                    .HasConstraintName("FK_ProductDiamond_Diamond");

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductDiamond_Product");
            });

            modelBuilder.Entity<ProductMaterial>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ProductMaterial");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Weight).HasColumnType("decimal(10, 4)");

                entity.HasOne(d => d.Material)
                    .WithMany()
                    .HasForeignKey(d => d.MaterialId)
                    .HasConstraintName("FK_ProductMaterial_Material");

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductMaterial_Product");
            });

            modelBuilder.Entity<WarrantyTicket>(entity =>
            {
                entity.HasKey(e => e.WarrantyId)
                    .HasName("PK__Warranty__2ED31813CD36240A");

                entity.HasIndex(e => e.ProductId, "UQ__Warranty__B40CC6EC5B809D60")
                    .IsUnique();

                entity.HasIndex(e => e.ProductId, "UQ__Warranty__B40CC6EC75FB568A")
                    .IsUnique();

                entity.Property(e => e.OrderSellDetailId).HasColumnName("OrderSellDetailID");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("ProductID");

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.WarrantyEndDate).HasColumnType("date");

                entity.Property(e => e.WarrantyStartDate).HasColumnType("date");

                entity.HasOne(d => d.OrderSellDetail)
                    .WithMany(p => p.WarrantyTickets)
                    .HasForeignKey(d => d.OrderSellDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WarrantyTickets_OrderDetail");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
