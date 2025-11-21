using Firmeza.Domain.Entities;
using Firmeza.Domain.Enums;
using Firmeza.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Mvc.Controllers;

[Authorize(Roles = "Admin")]
public class VehiclesController : Controller
{
    private readonly ApplicationDbContext _context;

    public VehiclesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Vehicles
    public async Task<IActionResult> Index()
    {
        var vehicles = await _context.Vehicles
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
        return View(vehicles);
    }

    // GET: Vehicles/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var vehicle = await _context.Vehicles
            .Include(v => v.Rentals)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (vehicle == null) return NotFound();

        return View(vehicle);
    }

    // GET: Vehicles/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Vehicles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Brand,Model,PlateNumber,Year,PricePerDay,Status,ImageUrl")] Vehicle vehicle)
    {
        if (ModelState.IsValid)
        {
            if (await _context.Vehicles.AnyAsync(v => v.PlateNumber == vehicle.PlateNumber))
            {
                ModelState.AddModelError("PlateNumber", "Ya existe un vehículo con esta placa.");
                return View(vehicle);
            }

            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.IsActive = true;
            
            _context.Add(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(vehicle);
    }

    // GET: Vehicles/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) return NotFound();
        
        return View(vehicle);
    }

    // POST: Vehicles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,PlateNumber,Year,PricePerDay,Status,ImageUrl,IsActive")] Vehicle vehicle)
    {
        if (id != vehicle.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                if (await _context.Vehicles.AnyAsync(v => v.PlateNumber == vehicle.PlateNumber && v.Id != id))
                {
                    ModelState.AddModelError("PlateNumber", "Ya existe otro vehículo con esta placa.");
                    return View(vehicle);
                }

                // Get existing vehicle to preserve CreatedAt
                var existingVehicle = await _context.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
                if (existingVehicle == null) return NotFound();

                // Preserve CreatedAt and set UpdatedAt
                vehicle.CreatedAt = existingVehicle.CreatedAt;
                vehicle.UpdatedAt = DateTime.UtcNow;
                
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(vehicle.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(vehicle);
    }

    // GET: Vehicles/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(m => m.Id == id);

        if (vehicle == null) return NotFound();

        return View(vehicle);
    }

    // POST: Vehicles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool VehicleExists(int id)
    {
        return _context.Vehicles.Any(e => e.Id == id);
    }
}
