using Microsoft.EntityFrameworkCore;
using MonRestoAPI.Models;

namespace MonRestoAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)        // Relation Article -> Category
                .WithMany()                      // Si Category peut avoir plusieurs Articles (One-to-Many)
                .HasForeignKey(a => a.CategoryId) // Clé étrangère sur CategoryId
                .OnDelete(DeleteBehavior.Restrict); // Optionnel : définir le comportement de suppression

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique(); // Empêche les doublons de noms d'utilisateur

            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Article)
                .WithMany()
                .HasForeignKey(o => o.ArticleId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(o => o.OrderId);

            // Configurations pour Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)            // Relation Payment -> User
                .WithMany()                     // Un User peut avoir plusieurs Payments (One-to-Many)
                .HasForeignKey(p => p.UserId)   // Clé étrangère sur UserId
                .OnDelete(DeleteBehavior.Cascade);






        }




    }
}
