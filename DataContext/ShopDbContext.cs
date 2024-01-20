using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Shop.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using System;

namespace Shop.DataContext
{
    public class ShopDbContext : IdentityDbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {

        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Discount> Discounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var dictionaryConverter = new ValueConverter<Dictionary<int, int>, string>(
                v => JsonSerializer.Serialize(v, null),
                v => JsonSerializer.Deserialize<Dictionary<int, int>>(v, null)
            );

            var dictionaryComparer = new ValueComparer<Dictionary<int, int>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => new Dictionary<int, int>(c)
            );

            modelBuilder.Entity<Cart>()
                .Property(e => e.Items)
                .HasConversion(dictionaryConverter);

            modelBuilder.Entity<Article>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryId);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Articles)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Article>()
                .Property(a => a.BarCode)
                .IsRequired();

            modelBuilder.Entity<Article>()
                .Property(a => a.ProductName)
                .IsRequired()
                .HasMaxLength(40);

            modelBuilder.Entity<Article>()
                .Property(a => a.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            modelBuilder.Entity<Category>()
                .Property(c => c.CategoryName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Articles)
                .WithOne(a => a.Category)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}