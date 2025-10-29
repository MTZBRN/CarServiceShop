using CarServiceShopBackend.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarServiceShopBackend.Models;

[Route("api/[controller]")]
[ApiController]
public class PartController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<PartController> _logger;

    public PartController(AppDbContext context, ILogger<PartController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Parts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Part>>> GetParts()
    {
        // 🔧 JAVÍTÁS: Ne Include-old, hogy elkerüljük a circular reference-t
        var parts = await _context.Parts.ToListAsync();
        return parts;
    }

    // GET: api/Parts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Part>> GetPart(int id)
    {
        var part = await _context.Parts.FindAsync(id);
        if (part == null) return NotFound();
        return part;
    }

    // GET: api/Parts/byservice/5 - Javított endpoint
    [HttpGet("byservice/{serviceId}")]
    public async Task<ActionResult<IEnumerable<Part>>> GetPartsByService(int serviceId)
    {
        try
        {
            _logger.LogInformation($"🔍 Getting parts for ServiceId: {serviceId}");

            // Ellenőrizzük, hogy létezik-e a Service
            var serviceExists = await _context.Services.AnyAsync(s => s.Id == serviceId);
            if (!serviceExists)
            {
                _logger.LogWarning($"⚠️ Service with ID {serviceId} not found");
                return Ok(new List<Part>()); // Üres lista visszaadása
            }

            // 🔧 JAVÍTÁS: Ne Include-old
            var parts = await _context.Parts
                .Where(p => p.ServiceId == serviceId)
                .ToListAsync();

            _logger.LogInformation($"✅ Found {parts.Count} parts for ServiceId: {serviceId}");
            return parts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error getting parts for ServiceId: {serviceId}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // POST: api/Parts
    [HttpPost]
    public async Task<ActionResult<Part>> PostPart(Part part)
    {
        try
        {
            _logger.LogInformation($"📝 Creating part: {part.Name}");

            // Ellenőrizzük, hogy létezik-e a Service
            var serviceExists = await _context.Services.AnyAsync(s => s.Id == part.ServiceId);
            if (!serviceExists)
            {
                _logger.LogWarning($"⚠️ Service with ID {part.ServiceId} not found");
                return BadRequest($"Service with ID {part.ServiceId} does not exist.");
            }

            _context.Parts.Add(part);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"✅ Part created successfully with ID: {part.Id}");
            return CreatedAtAction(nameof(GetPart), new { id = part.Id }, part);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error creating part");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // PUT: api/Parts/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPart(int id, Part part)
    {
        if (id != part.Id) return BadRequest();

        try
        {
            // Ellenőrizzük, hogy létezik-e a Service
            var serviceExists = await _context.Services.AnyAsync(s => s.Id == part.ServiceId);
            if (!serviceExists)
            {
                return BadRequest($"Service with ID {part.ServiceId} does not exist.");
            }

            _context.Entry(part).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PartExists(id))
            {
                return NotFound();
            }
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating part {id}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // DELETE: api/Parts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePart(int id)
    {
        try
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) return NotFound();

            _context.Parts.Remove(part);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting part {id}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    private bool PartExists(int id)
    {
        return _context.Parts.Any(e => e.Id == id);
    }
}
