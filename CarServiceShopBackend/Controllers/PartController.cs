using CarServiceShopBackend.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarServiceShopBackend.Models;

[Route("api/[controller]")]
[ApiController]
public class PartController : ControllerBase
{
    private readonly AppDbContext _context;

    public PartController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Parts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Part>>> GetParts()
    {
        return await _context.Parts.ToListAsync();
    }

    // GET: api/Parts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Part>> GetPart(int id)
    {
        var part = await _context.Parts.FindAsync(id);
        if (part == null) return NotFound();
        return part;
    }

    // POST: api/Parts
    [HttpPost]
    public async Task<ActionResult<Part>> PostPart(Part part)
    {
        _context.Parts.Add(part);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPart), new { id = part.Id }, part);
    }

    // PUT: api/Parts/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPart(int id, Part part)
    {
        if (id != part.Id) return BadRequest();
        _context.Entry(part).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Parts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePart(int id)
    {
        var part = await _context.Parts.FindAsync(id);
        if (part == null) return NotFound();
        _context.Parts.Remove(part);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}