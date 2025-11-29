using Firmeza.Domain.Entities;
using Firmeza.Domain.Enums;
using Firmeza.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Data;

public class DbSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public DbSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task SeedAsync()
    {
        // 0. Apply migrations
        await _context.Database.MigrateAsync();

        // 1. Seed Roles
        await SeedRolesAsync();

        // 2. Seed Admin User
        await SeedAdminUserAsync();

        // 3. Seed Categories and Products
        await SeedCatalogAsync();
    }

    private async Task SeedRolesAsync()
    {
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await _roleManager.RoleExistsAsync("Customer"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Customer"));
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var adminEmail = "admin@firmeza.com";
        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "Principal",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(adminUser, "Admin123!");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    private async Task SeedCatalogAsync()
    {
        // Check if we already have categories
        if (await _context.Categories.AnyAsync())
            return;

        // Create Categories
        var catMateriales = new Category { Name = "Materiales de Construcción", Description = "Cemento, arena, ladrillos, etc." };
        var catHerramientas = new Category { Name = "Herramientas", Description = "Herramientas manuales y eléctricas" };
        var catAcabados = new Category { Name = "Acabados", Description = "Pinturas, cerámicas, pisos" };

        _context.Categories.AddRange(catMateriales, catHerramientas, catAcabados);
        await _context.SaveChangesAsync();

        // Create Products
        var products = new List<Product>
        {
            new Product
            {
                Name = "Cemento Portland Tipo 1",
                Description = "Cemento gris de uso general para construcción",
                Code = "MAT-001",
                Price = 28500,
                Stock = 500,
                Type = ProductType.Material,
                CategoryId = catMateriales.Id
            },
            new Product
            {
                Name = "Ladrillo Tolete",
                Description = "Ladrillo de arcilla cocida para mampostería",
                Code = "MAT-002",
                Price = 1200,
                Stock = 10000,
                Type = ProductType.Material,
                CategoryId = catMateriales.Id
            },
            new Product
            {
                Name = "Taladro Percutor 1/2\"",
                Description = "Taladro industrial 800W con velocidad variable",
                Code = "HER-001",
                Price = 250000,
                Stock = 15,
                Type = ProductType.Tool,
                CategoryId = catHerramientas.Id
            },
            new Product
            {
                Name = "Juego de Destornilladores",
                Description = "Set de 6 destornilladores punta plana y estrella",
                Code = "HER-002",
                Price = 45000,
                Stock = 30,
                Type = ProductType.Tool,
                CategoryId = catHerramientas.Id
            }
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();
    }
}
