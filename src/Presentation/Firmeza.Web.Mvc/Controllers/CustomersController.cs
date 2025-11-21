using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Mvc.Controllers;

[Authorize(Roles = "Admin")]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Customers
    public async Task<IActionResult> Index()
    {
        var customers = await _context.Customers
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return View(customers);
    }

    // GET: Customers/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var customer = await _context.Customers
            .Include(c => c.Sales)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (customer == null) return NotFound();

        return View(customer);
    }

    // GET: Customers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Customers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,DocumentType,DocumentNumber,Email,Phone,Address,City")] Customer customer)
    {
        if (ModelState.IsValid)
        {
            // Validate Unique Document
            if (await _context.Customers.AnyAsync(c => c.DocumentNumber == customer.DocumentNumber))
            {
                ModelState.AddModelError("DocumentNumber", "Ya existe un cliente con este número de documento.");
                return View(customer);
            }

            // Validate Unique Email
            if (await _context.Customers.AnyAsync(c => c.Email == customer.Email))
            {
                ModelState.AddModelError("Email", "Ya existe un cliente con este correo electrónico.");
                return View(customer);
            }

            customer.CreatedAt = DateTime.UtcNow;
            customer.IsActive = true;
            
            _context.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    // GET: Customers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();
        
        return View(customer);
    }

    // POST: Customers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,DocumentType,DocumentNumber,Email,Phone,Address,City,IsActive,UserId")] Customer customer)
    {
        if (id != customer.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                // Validate Unique Document (excluding current customer)
                if (await _context.Customers.AnyAsync(c => c.DocumentNumber == customer.DocumentNumber && c.Id != id))
                {
                    ModelState.AddModelError("DocumentNumber", "Ya existe otro cliente con este número de documento.");
                    return View(customer);
                }

                // Validate Unique Email (excluding current customer)
                if (await _context.Customers.AnyAsync(c => c.Email == customer.Email && c.Id != id))
                {
                    ModelState.AddModelError("Email", "Ya existe otro cliente con este correo electrónico.");
                    return View(customer);
                }

                // Get existing customer to preserve CreatedAt
                var existingCustomer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                if (existingCustomer == null) return NotFound();

                // Preserve CreatedAt and set UpdatedAt
                customer.CreatedAt = existingCustomer.CreatedAt;
                customer.UpdatedAt = DateTime.UtcNow;
                
                _context.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    // GET: Customers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var customer = await _context.Customers
            .FirstOrDefaultAsync(m => m.Id == id);

        if (customer == null) return NotFound();

        return View(customer);
    }

    // POST: Customers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            // Check for dependencies
            var hasSales = await _context.Sales.AnyAsync(s => s.CustomerId == id);
            if (hasSales)
            {
                // Soft delete instead
                customer.IsActive = false;
                customer.UpdatedAt = DateTime.UtcNow;
                _context.Update(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool CustomerExists(int id)
    {
        return _context.Customers.Any(e => e.Id == id);
    }
}
