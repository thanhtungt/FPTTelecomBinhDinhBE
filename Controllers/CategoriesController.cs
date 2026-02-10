using AutoMapper;
using FPTTelecomBE.Data;
using FPTTelecomBE.DTOs.Category;
using FPTTelecomBE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FPTTelecomBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(AppDbContext context, IMapper mapper, ILogger<CategoriesController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/categories (Public - Chỉ active categories)
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .Where(c => c.Active)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                DisplayOrder = c.DisplayOrder,
                Active = c.Active,
                PackageCount = c.Packages.Count(p => p.Active)
            })
            .ToListAsync();

        return Ok(categories);
    }

    // GET: api/categories/all (Admin - Tất cả categories)
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                DisplayOrder = c.DisplayOrder,
                Active = c.Active,
                PackageCount = c.Packages.Count
            })
            .ToListAsync();

        _logger.LogInformation("Admin retrieved all {Count} categories", categories.Count);
        return Ok(categories);
    }

    // GET: api/categories/{id} (Public)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Packages.Where(p => p.Active))
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound(new { message = "Không tìm thấy danh mục" });

        var result = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            DisplayOrder = category.DisplayOrder,
            Active = category.Active,
            PackageCount = category.Packages.Count
        };

        return Ok(result);
    }

    // GET: api/categories/slug/{slug} (Public)
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetCategoryBySlug(string slug)
    {
        var category = await _context.Categories
            .Include(c => c.Packages.Where(p => p.Active))
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (category == null)
            return NotFound(new { message = "Không tìm thấy danh mục" });

        var result = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            DisplayOrder = category.DisplayOrder,
            Active = category.Active,
            PackageCount = category.Packages.Count
        };

        return Ok(result);
    }

    // POST: api/categories (Admin - Tạo category mới)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        // Kiểm tra tên category đã tồn tại chưa
        var slug = GenerateSlug(dto.Name);
        var existingCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (existingCategory != null)
        {
            return BadRequest(new { message = "Tên danh mục đã tồn tại" });
        }

        var category = new Category
        {
            Name = dto.Name,
            Slug = slug,
            DisplayOrder = dto.DisplayOrder,
            Active = dto.Active,
            CreatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var result = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            DisplayOrder = category.DisplayOrder,
            Active = category.Active,
            PackageCount = 0
        };

        _logger.LogInformation("Admin created new category: {CategoryId} - {Name}", category.Id, category.Name);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, result);
    }

    // PUT: api/categories/{id} (Admin - Cập nhật category)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound(new { message = "Không tìm thấy danh mục" });

        // Kiểm tra tên category trùng
        var slug = GenerateSlug(dto.Name);
        var existingCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Slug == slug && c.Id != id);

        if (existingCategory != null)
        {
            return BadRequest(new { message = "Tên danh mục đã tồn tại" });
        }

        category.Name = dto.Name;
        category.Slug = slug;
        category.DisplayOrder = dto.DisplayOrder;
        category.Active = dto.Active;

        await _context.SaveChangesAsync();

        var packageCount = await _context.Packages.CountAsync(p => p.CategoryId == id);

        var result = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            DisplayOrder = category.DisplayOrder,
            Active = category.Active,
            PackageCount = packageCount
        };

        _logger.LogInformation("Admin updated category: {CategoryId} - {Name}", category.Id, category.Name);
        return Ok(result);
    }

    // DELETE: api/categories/{id} (Admin - Xóa category)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Packages)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound(new { message = "Không tìm thấy danh mục" });

        // Kiểm tra xem có packages nào trong category này không
        if (category.Packages.Any())
        {
            return BadRequest(new
            {
                message = $"Không thể xóa danh mục này vì đang có {category.Packages.Count} gói cước. Hãy chuyển các gói cước sang danh mục khác hoặc xóa chúng trước."
            });
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Admin deleted category: {CategoryId} - {Name}", id, category.Name);
        return NoContent();
    }

    // PATCH: api/categories/{id}/toggle-active (Admin - Bật/tắt category)
    [HttpPatch("{id}/toggle-active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound(new { message = "Không tìm thấy danh mục" });

        category.Active = !category.Active;
        await _context.SaveChangesAsync();

        var packageCount = await _context.Packages.CountAsync(p => p.CategoryId == id);

        var result = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            DisplayOrder = category.DisplayOrder,
            Active = category.Active,
            PackageCount = packageCount
        };

        _logger.LogInformation("Admin toggled category {CategoryId} active status to {Active}", id, category.Active);
        return Ok(result);
    }

    // Helper method: Generate slug from name
    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant();

        // Vietnamese character mapping
        slug = Regex.Replace(slug, "[áàảãạâấầẩẫậăắằẳẵặ]", "a");
        slug = Regex.Replace(slug, "[éèẻẽẹêếềểễệ]", "e");
        slug = Regex.Replace(slug, "[íìỉĩị]", "i");
        slug = Regex.Replace(slug, "[óòỏõọôốồổỗộơớờởỡợ]", "o");
        slug = Regex.Replace(slug, "[úùủũụưứừửữự]", "u");
        slug = Regex.Replace(slug, "[ýỳỷỹỵ]", "y");
        slug = Regex.Replace(slug, "đ", "d");

        // Remove invalid chars
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // Convert multiple spaces/hyphens into one
        slug = Regex.Replace(slug, @"[\s-]+", " ").Trim();

        // Replace spaces with hyphens
        slug = Regex.Replace(slug, @"\s", "-");

        return slug;
    }
}