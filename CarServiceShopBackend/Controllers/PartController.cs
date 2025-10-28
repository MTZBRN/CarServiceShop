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
        return await _context.Parts
            .Include(p => p.Service)
            .ToListAsync();
    }

    // GET: api/Parts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Part>> GetPart(int id)
    {
        var part = await _context.Parts
            .Include(p => p.Service)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (part == null) return NotFound();
        return part;
    }

    // GET: api/Parts/byservice/5 - Új endpoint!
    [HttpGet("byservice/{serviceId}")]
    public async Task<ActionResult<IEnumerable<Part>>> GetPartsByService(int serviceId)
    {
        var parts = await _context.Parts
            .Where(p => p.ServiceId == serviceId)
            .Include(p => p.Service)
            .ToListAsync();
        
        return parts;
    }

    // POST: api/Parts
    [HttpPost]
    public async Task<ActionResult<Part>> PostPart(Part part)
    {
        // Ellenőrizzük, hogy létezik-e a Service
        var serviceExists = await _context.Services.AnyAsync(s => s.Id == part.ServiceId);
        if (!serviceExists)
        {
            return BadRequest($"Service with ID {part.ServiceId} does not exist.");
        }

        _context.Parts.Add(part);
        await _context.SaveChangesAsync();
        
        // Visszaadjuk a teljes objektumot a Service navigációs tulajdonsággal
        var createdPart = await _context.Parts
            .Include(p => p.Service)
            .FirstOrDefaultAsync(p => p.Id == part.Id);
            
        return CreatedAtAction(nameof(GetPart), new { id = part.Id }, createdPart);
    }

    // PUT: api/Parts/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPart(int id, Part part)
    {
        if (id != part.Id) return BadRequest();
        
        // Ellenőrizzük, hogy létezik-e a Service
        var serviceExists = await _context.Services.AnyAsync(s => s.Id == part.ServiceId);
        if (!serviceExists)
        {
            return BadRequest($"Service with ID {part.ServiceId} does not exist.");
        }

        _context.Entry(part).State = EntityState.Modified;
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PartExists(id))
            {
                return NotFound();
            }
            throw;
        }
        
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

    private bool PartExists(int id)
    {
        return _context.Parts.Any(e => e.Id == id);
    }
}