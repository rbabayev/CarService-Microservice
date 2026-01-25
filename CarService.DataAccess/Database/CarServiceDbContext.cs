using CarService.Entities.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CarService.DataAccess.Database
{
    public class CarServiceDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {

        public CarServiceDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<AuctionProduct> AuctionProducts { get; set; }
        public DbSet<AuctionBid> AuctionBids { get; set; }
        public DbSet<AuctionTransaction> AuctionTransactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<User>()
                .HasOne(u => u.Shop)
                .WithOne(s => s.User)
                .HasForeignKey<Shop>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Worker>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuctionBid>()
                .HasOne(b => b.Product)
                .WithMany(p => p.Bids)
                .HasForeignKey(b => b.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuctionBid>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔧 AuctionTransaction
            modelBuilder.Entity<AuctionTransaction>()
                .HasOne(at => at.AuctionProduct)
                .WithMany()
                .HasForeignKey(at => at.AuctionProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuctionTransaction>()
                .HasOne(at => at.WinnerUser)
                .WithMany()
                .HasForeignKey(at => at.WinnerUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
