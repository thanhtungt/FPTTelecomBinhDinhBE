using FPTTelecomBE.Models;
using FPTWifiBE.Models;
using Microsoft.EntityFrameworkCore;

namespace FPTTelecomBE.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure decimal precision for Package.PriceMonthly
        modelBuilder.Entity<Package>()
            .Property(p => p.PriceMonthly)
            .HasPrecision(18, 2);

        // Category -> Package relationship
        modelBuilder.Entity<Package>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Packages)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); // Không cho xóa category nếu còn packages

        // Registration relationships
        modelBuilder.Entity<Registration>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.NoAction);

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

        // Seed data
        SeedUsers(modelBuilder);
        SeedCategories(modelBuilder);
        SeedPackages(modelBuilder);
    }

    private static void SeedUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Admin FPT Bình Định",
                Email = "admin@fptbinhdinh.com",
                Phone = "0332766193",
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
    }

    private static void SeedCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Internet Gia Đình",
                Slug = "internet-gia-dinh",
                Description = "Các gói cước internet dành cho hộ gia đình với tốc độ và giá cả đa dạng.",
                DisplayOrder = 1,
                Active = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = 2,
                Name = "Combo Đa Dịch Vụ",
                Slug = "combo-da-dich-vu",
                Description = "Các gói combo tích hợp internet với truyền hình, điện thoại và các dịch vụ khác.",
                DisplayOrder = 2,
                Active = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = 3,
                Name = "WiFi 7 Cao Cấp",
                Slug = "wifi-7-cao-cap",
                Description = "Các gói cước sử dụng công nghệ WiFi 7 mới nhất với tốc độ siêu nhanh và ổn định.",
                DisplayOrder = 3,
                Active = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = 4,
                Name = "Doanh Nghiệp",
                Slug = "doanh-nghiep",
                Description = "Các gói cước internet và dịch vụ dành riêng cho doanh nghiệp với yêu cầu cao về tốc độ và độ ổn định.",
                DisplayOrder = 4,
                Active = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }

    private static void SeedPackages(ModelBuilder modelBuilder)
    {
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
                CategoryId = 1, // Internet Gia Đình
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
                CategoryId = 2, // Combo
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
                CategoryId = 1, // Internet Gia Đình
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
                CategoryId = 2, // Combo
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
                CategoryId = 3, // WiFi 7
                Active = true
            }
        );
    }
}