using Microservices.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservices.Services.CouponAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Coupon>().HasData(
            new Coupon { CouponId = 1, CouponCode = "SAVE10", DiscountAmount = 10.0, MinAmount = 20 },
            new Coupon { CouponId = 2, CouponCode = "SAVE20", DiscountAmount = 20.0, MinAmount = 40 }
        );
    }
}
