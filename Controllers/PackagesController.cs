using AutoMapper;
using FPTTelecomBE.Data;
using FPTTelecomBE.DTOs.Package;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FPTTelecomBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PackagesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<PackagesController> _logger;

    public PackagesController(AppDbContext context, IMapper mapper, ILogger<PackagesController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/packages?categoryId=1 (Public - Filter theo categoryId)
    [HttpGet]
    public async Task<IActionResult> GetPackages([FromQuery] int? categoryId = null)
    {
        var query = _context.Packages
            .Include(p => p.Category)
            .Where(p => p.Active);

        // Filter theo category nếu có
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
            _logger.LogInformation("Filtering packages by categoryId: {CategoryId}", categoryId.Value);
        }

        var packages = await query
            .OrderBy(p => p.PriceMonthly)
            .Select(p => new PackageDto
            {
                Id = p.Id,
                Name = p.Name,
                SpeedDown = p.SpeedDown,
                SpeedUp = p.SpeedUp,
                PriceMonthly = p.PriceMonthly,
                PromotionText = p.PromotionText,
                DeviceBonus = p.DeviceBonus,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category!.Name,
                CategorySlug = p.Category.Slug,
                Active = p.Active
            })
            .ToListAsync();

        return Ok(packages);
    }

    // GET: api/packages/all?categoryId=1 (Admin - Filter tất cả packages)
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllPackages([FromQuery] int? categoryId = null)
    {
        var query = _context.Packages
            .Include(p => p.Category)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        var packages = await query
            .OrderBy(p => p.PriceMonthly)
            .Select(p => new PackageDto
            {
                Id = p.Id,
                Name = p.Name,
                SpeedDown = p.SpeedDown,
                SpeedUp = p.SpeedUp,
                PriceMonthly = p.PriceMonthly,
                PromotionText = p.PromotionText,
                DeviceBonus = p.DeviceBonus,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category!.Name,
                CategorySlug = p.Category.Slug,
                Active = p.Active
            })
            .ToListAsync();

        _logger.LogInformation("Admin retrieved {Count} packages", packages.Count);
        return Ok(packages);
    }

    // GET: api/packages/{id} (Public)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPackageById(int id)
    {
        var package = await _context.Packages
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (package == null)
            return NotFound(new { message = "Không tìm thấy gói cước" });

        var result = new PackageDto
        {
            Id = package.Id,
            Name = package.Name,
            SpeedDown = package.SpeedDown,
            SpeedUp = package.SpeedUp,
            PriceMonthly = package.PriceMonthly,
            PromotionText = package.PromotionText,
            DeviceBonus = package.DeviceBonus,
            ImageUrl = package.ImageUrl,
            CategoryId = package.CategoryId,
            CategoryName = package.Category?.Name ?? "",
            CategorySlug = package.Category?.Slug ?? "",
            Active = package.Active
        };

        return Ok(result);
    }

    // POST: api/packages (Admin - Tạo package mới)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePackage([FromBody] CreatePackageDto dto)
    {
        // Kiểm tra category có tồn tại không
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
        {
            return BadRequest(new { message = "Danh mục không tồn tại" });
        }

        // Kiểm tra tên gói cước đã tồn tại chưa
        var existingPackage = await _context.Packages
            .FirstOrDefaultAsync(p => p.Name.ToLower() == dto.Name.ToLower());

        if (existingPackage != null)
        {
            return BadRequest(new { message = "Tên gói cước đã tồn tại" });
        }

        var package = new Models.Package
        {
            Name = dto.Name,
            SpeedDown = dto.SpeedDown,
            SpeedUp = dto.SpeedUp,
            PriceMonthly = dto.PriceMonthly,
            PromotionText = dto.PromotionText,
            DeviceBonus = dto.DeviceBonus,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId,
            Active = dto.Active
        };

        _context.Packages.Add(package);
        await _context.SaveChangesAsync();

        // Load category info
        await _context.Entry(package).Reference(p => p.Category).LoadAsync();

        var result = new PackageDto
        {
            Id = package.Id,
            Name = package.Name,
            SpeedDown = package.SpeedDown,
            SpeedUp = package.SpeedUp,
            PriceMonthly = package.PriceMonthly,
            PromotionText = package.PromotionText,
            DeviceBonus = package.DeviceBonus,
            ImageUrl = package.ImageUrl,
            CategoryId = package.CategoryId,
            CategoryName = package.Category?.Name ?? "",
            CategorySlug = package.Category?.Slug ?? "",
            Active = package.Active
        };

        _logger.LogInformation("Admin created new package: {PackageId} - {Name} - Category: {CategoryId}",
            package.Id, package.Name, package.CategoryId);

        return CreatedAtAction(nameof(GetPackageById), new { id = package.Id }, result);
    }

    // PUT: api/packages/{id} (Admin - Cập nhật package)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePackage(int id, [FromBody] UpdatePackageDto dto)
    {
        var package = await _context.Packages
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (package == null)
            return NotFound(new { message = "Không tìm thấy gói cước" });

        // Kiểm tra category có tồn tại không
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
        {
            return BadRequest(new { message = "Danh mục không tồn tại" });
        }

        // Kiểm tra tên gói cước trùng
        var existingPackage = await _context.Packages
            .FirstOrDefaultAsync(p => p.Name.ToLower() == dto.Name.ToLower() && p.Id != id);

        if (existingPackage != null)
        {
            return BadRequest(new { message = "Tên gói cước đã tồn tại" });
        }

        package.Name = dto.Name;
        package.SpeedDown = dto.SpeedDown;
        package.SpeedUp = dto.SpeedUp;
        package.PriceMonthly = dto.PriceMonthly;
        package.PromotionText = dto.PromotionText;
        package.DeviceBonus = dto.DeviceBonus;
        package.ImageUrl = dto.ImageUrl;
        package.CategoryId = dto.CategoryId;
        package.Active = dto.Active;

        await _context.SaveChangesAsync();

        // Reload category if changed
        await _context.Entry(package).Reference(p => p.Category).LoadAsync();

        var result = new PackageDto
        {
            Id = package.Id,
            Name = package.Name,
            SpeedDown = package.SpeedDown,
            SpeedUp = package.SpeedUp,
            PriceMonthly = package.PriceMonthly,
            PromotionText = package.PromotionText,
            DeviceBonus = package.DeviceBonus,
            ImageUrl = package.ImageUrl,
            CategoryId = package.CategoryId,
            CategoryName = package.Category?.Name ?? "",
            CategorySlug = package.Category?.Slug ?? "",
            Active = package.Active
        };

        _logger.LogInformation("Admin updated package: {PackageId} - {Name} - Category: {CategoryId}",
            package.Id, package.Name, package.CategoryId);

        return Ok(result);
    }

    // DELETE: api/packages/{id} (Admin - Xóa package)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePackage(int id)
    {
        var package = await _context.Packages.FindAsync(id);
        if (package == null)
            return NotFound(new { message = "Không tìm thấy gói cước" });

        // Kiểm tra đơn đăng ký
        var hasRegistrations = await _context.Registrations
            .AnyAsync(r => r.PackageId == id);

        if (hasRegistrations)
        {
            return BadRequest(new
            {
                message = "Không thể xóa gói cước này vì đang có đơn đăng ký sử dụng."
            });
        }

        _context.Packages.Remove(package);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Admin deleted package: {PackageId} - {Name}", id, package.Name);
        return NoContent();
    }

    // PATCH: api/packages/{id}/toggle-active (Admin - Bật/tắt package)
    [HttpPatch("{id}/toggle-active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var package = await _context.Packages
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (package == null)
            return NotFound(new { message = "Không tìm thấy gói cước" });

        package.Active = !package.Active;
        await _context.SaveChangesAsync();

        var result = new PackageDto
        {
            Id = package.Id,
            Name = package.Name,
            SpeedDown = package.SpeedDown,
            SpeedUp = package.SpeedUp,
            PriceMonthly = package.PriceMonthly,
            PromotionText = package.PromotionText,
            DeviceBonus = package.DeviceBonus,
            ImageUrl = package.ImageUrl,
            CategoryId = package.CategoryId,
            CategoryName = package.Category?.Name ?? "",
            CategorySlug = package.Category?.Slug ?? "",
            Active = package.Active
        };

        _logger.LogInformation("Admin toggled package {PackageId} active status to {Active}", id, package.Active);
        return Ok(result);
    }
}