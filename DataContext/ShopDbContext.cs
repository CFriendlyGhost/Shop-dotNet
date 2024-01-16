using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Shop.Models;

namespace Shop.DataContext
{
    public class ShopDbContext : IdentityDbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {

        }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<Article>()
                .Property(a => a.CountryOfOrigin)
                .HasMaxLength(50);

            modelBuilder.Entity<Article>()
                .Property(a => a.Weight)
                .IsRequired()
                .HasPrecision(18, 2);

            modelBuilder.Entity<Article>()
                .Property(a => a.ExpirationDate)
                .IsRequired();

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