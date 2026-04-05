using Microsoft.EntityFrameworkCore;
using EColor.API.Models;

namespace EColor.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ProductionOrder> ProductionOrders { get; set; }
        public DbSet<ProductionOrderItem> ProductionOrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置單號唯一索引
            modelBuilder.Entity<ProductionOrder>()
                .HasIndex(p => p.OrderNo)
                .IsUnique();

            // 配置外鍵關聯
            modelBuilder.Entity<ProductionOrder>()
                .HasMany(p => p.Items)
                .WithOne(i => i.ProductOrder)
                .HasForeignKey(i => i.ProductionOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
