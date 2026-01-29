using AutoMapper;
using FPTTelecomBE.Data;
using FPTTelecomBE.DTOs.Package;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FPTTelecomBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PackagesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PackagesController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/packages (Public)
    [HttpGet]
    public async Task<IActionResult> GetPackages()
    {
        var packages = await _context.Packages
            .Where(p => p.Active)
            .OrderBy(p => p.PriceMonthly)
            .ToListAsync();

        var result = _mapper.Map<List<PackageDto>>(packages);
        return Ok(result);
    }

    // GET: api/packages/{id} (Public)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPackageById(int id)
    {
        var package = await _context.Packages.FindAsync(id);
        if (package == null)
            return NotFound(new { message = "Không tìm thấy gói cước" });

        var result = _mapper.Map<PackageDto>(package);
        return Ok(result);
    }
}