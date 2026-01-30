using FPTTelecomBE.Models;
using FPTWifiBE.Models;
using Microsoft.EntityFrameworkCore;

namespace FPTTelecomBE.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure decimal precision for Package.PriceMonthly
        modelBuilder.Entity<Package>()
            .Property(p => p.PriceMonthly)
            .HasPrecision(18, 2);

        // Relationships
        modelBuilder.Entity<Registration>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.NoAction); // CHANGED: from SetNull to NoAction

        modelBuilder.Entity<Registration>()
            .HasOne(r => r.Package)
            .WithMany()
            .HasForeignKey(r => r.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Registration>()
            .HasOne(r => r.AssignedStaff)
            .WithMany()
            .HasForeignKey(r => r.AssignedStaffId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Admin FPT Bình Định",
                Email = "admin@fptbinhdinh.com",
                Phone = "0901234567",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 3,
                Name = "Staff Quy Nhơn",
                Email = "staff@fptbinhdinh.com",
                Phone = "0902345678",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@123"),
                Role = "Staff",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed packages thực tế 01/2026 (dựa trên fpt.vn và Bình Định site)
        modelBuilder.Entity<Package>().HasData(
            new Package
            {
                Id = 1,
                Name = "Internet Giga",
                SpeedDown = 300,
                SpeedUp = 300,
                PriceMonthly = 180000,
                PromotionText = "Tặng Modem WiFi 6 + Giảm 50k online + Tặng 1 tháng nếu trả trước 12 tháng",
                DeviceBonus = "Modem WiFi 6",
                Active = true
            },
            new Package
            {
                Id = 2,
                Name = "Internet Sky",
                SpeedDown = 1000,
                SpeedUp = 300,
                PriceMonthly = 190000,
                PromotionText = "Tặng Modem WiFi 6 + FPT Play Box (combo) + Giảm 50k",
                DeviceBonus = "Modem WiFi 6 + FPT Play Box",
                Active = true
            },
            new Package
            {
                Id = 3,
                Name = "Internet Meta",
                SpeedDown = 1000,
                SpeedUp = 1000,
                PriceMonthly = 305000,
                PromotionText = "Symmetric 1Gbps + WiFi 6 + Mesh/AP tùy F1/F2/F3 + Tặng tháng trả trước",
                DeviceBonus = "Modem WiFi 6 + Access Point/Mesh",
                Active = true
            },
            new Package
            {
                Id = 4,
                Name = "Combo Thể Thao Sky",
                SpeedDown = 1000,
                SpeedUp = 300,
                PriceMonthly = 269000,
                PromotionText = "Ngoại hạng Anh + FPT Play Box + Modem WiFi 6",
                DeviceBonus = "Modem WiFi 6 + FPT Play Box",
                Active = true
            },
            new Package
            {
                Id = 5,
                Name = "SpeedX2 Pro (WiFi 7)",
                SpeedDown = 2000,
                SpeedUp = 2000,
                PriceMonthly = 1099000,
                PromotionText = "XGS-PON + WiFi 7 + Mesh WiFi 7 + FPT Play VIP",
                DeviceBonus = "Modem WiFi 7 + 1 Mesh WiFi 7",
                Active = true
            }
        );
    }
}