using AutoMapper;
using FPTTelecomBE.Data;
using FPTTelecomBE.DTOs.Post;
using FPTTelecomBE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FPTTelecomBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PostsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/posts (Public)
    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] string? category = null)
    {
        var query = _context.Posts.AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category == category);

        var posts = await query
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();

        var result = _mapper.Map<List<PostDto>>(posts);
        return Ok(result);
    }

    // GET: api/posts/{id} (Public)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            return NotFound(new { message = "Không tìm thấy bài viết" });

        var result = _mapper.Map<PostDto>(post);
        return Ok(result);
    }

    // GET: api/posts/slug/{slug} (Public)
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetPostBySlug(string slug)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
        if (post == null)
            return NotFound(new { message = "Không tìm thấy bài viết" });

        var result = _mapper.Map<PostDto>(post);
        return Ok(result);
    }

    // POST: api/posts (Admin only)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto dto)
    {
        var post = new Post
        {
            Title = dto.Title,
            Slug = GenerateSlug(dto.Title),
            Content = dto.Content,
            ImageUrl = dto.ImageUrl,
            Category = dto.Category,
            PublishedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<PostDto>(post);
        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, result);
    }

    // PUT: api/posts/{id} (Admin only)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] CreatePostDto dto)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            return NotFound(new { message = "Không tìm thấy bài viết" });

        post.Title = dto.Title;
        post.Slug = GenerateSlug(dto.Title);
        post.Content = dto.Content;
        post.ImageUrl = dto.ImageUrl;
        post.Category = dto.Category;

        await _context.SaveChangesAsync();

        var result = _mapper.Map<PostDto>(post);
        return Ok(result);
    }

    // DELETE: api/posts/{id} (Admin only)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            return NotFound(new { message = "Không tìm thấy bài viết" });

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static string GenerateSlug(string title)
    {
        // Convert to lowercase and remove diacritics
        var slug = title.ToLowerInvariant();

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