using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Data;
using Firmeza.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ApplicationDbContext context,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
        _emailService = emailService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(new { message = "Credenciales inválidas" });

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Credenciales inválidas" });

        // Check if user has a customer account and if it's active
        if (user.CustomerId.HasValue)
        {
            var customer = await _context.Customers.FindAsync(user.CustomerId.Value);
            if (customer == null || !customer.IsActive)
                return Unauthorized(new { message = "Cuenta inactiva. Contacte al administrador." });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user.Id, user.Email!, user.FirstName ?? "", user.LastName ?? "", roles);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            CustomerId = user.CustomerId,
            Roles = roles.ToList()
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        // Check if email already exists
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return BadRequest(new { message = "El correo ya está registrado" });

        // Check if document already exists
        if (await _context.Customers.AnyAsync(c => c.DocumentNumber == request.DocumentNumber))
            return BadRequest(new { message = "El documento ya está registrado" });

        // Create ApplicationUser
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(new { message = "Error al crear el usuario", errors = result.Errors });

        // Assign Customer role
        await _userManager.AddToRoleAsync(user, "Customer");

        // Create Customer entity
        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        // Update user with CustomerId
        user.CustomerId = customer.Id;
        await _userManager.UpdateAsync(user);

        // Send welcome email
        try
        {
            await _emailService.SendWelcomeEmailAsync(user.Email!, customer.FullName);
        }
        catch (Exception ex)
        {
            // Log error but don't fail registration
            // Email sending is not critical for registration success
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user.Id, user.Email!, user.FirstName ?? "", user.LastName ?? "", roles);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            CustomerId = customer.Id,
            Roles = roles.ToList()
        });
    }
}
