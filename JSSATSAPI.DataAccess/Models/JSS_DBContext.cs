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
        public virtual DbSet<DiamondPrice> DiamondPrices { get; set; } = null!;
        public virtual DbSet<Material> Materials { get; set; } = null!;
        public virtual DbSet<MaterialPrice> MaterialPrices { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

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

                entity.HasIndex(e => e.AccountId, "UQ__Counter__349DA5A7E944DDAB")
                    .IsUnique();

                entity.Property(e => e.CounterName).HasMaxLength(255);

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.Counter)
                    .HasForeignKey<Counter>(d => d.AccountId)
                    .HasConstraintName("FK_Counter_Account");
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
