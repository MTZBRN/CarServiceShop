using CarServiceShopBackend.DbContext;
using CarServiceShopBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarServiceShopBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarController(AppDbContext context) : ControllerBase
{
    // GET: api/Cars
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Car>>> GetCars()
    {
        return await context.Cars.ToListAsync();
    }

    // GET: api/Cars/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Car>> GetCar(int id)
    {
        var car = await context.Cars.FindAsync(id);
        if (car == null) return NotFound();
        return car;
    }

    // POST: api/Cars
    [HttpPost]
    public async Task<ActionResult<Car>> PostCar(Car car)
    {
        context.Cars.Add(car);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
    }

    // PUT: api/Cars/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCar(int id, Car car)
    {
        if (id != car.Id) return BadRequest();
        context.Entry(car).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Cars/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCar(int id)
    {
        var car = await context.Cars.FindAsync(id);
        if (car == null) return NotFound();
        context.Cars.Remove(car);
        await context.SaveChangesAsync();
        return NoContent();
    }
}