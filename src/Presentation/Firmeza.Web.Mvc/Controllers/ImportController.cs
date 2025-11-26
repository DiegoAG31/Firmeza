using Firmeza.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Web.Mvc.Controllers;

[Authorize(Roles = "Admin")]
public class ImportController : Controller
{
    private readonly IExcelImportService _importService;

    public ImportController(IExcelImportService importService)
    {
        _importService = importService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        // Configure EPPlus License
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("", "Por favor seleccione un archivo válido.");
            return View("Index");
        }

        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("", "Solo se permiten archivos Excel (.xlsx).");
            return View("Index");
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _importService.ImportDataAsync(stream);
            
            ViewBag.SuccessMessage = $"Importación completada: {result.SuccessCount} registros procesados correctamente.";
            if (result.ErrorCount > 0)
            {
                ViewBag.ErrorMessage = $"Se encontraron {result.ErrorCount} errores.";
                ViewBag.Errors = result.Errors;
            }
            
            return View("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error durante la importación: {ex.Message}");
            ViewBag.ErrorDetails = ex.ToString();
            return View("Index");
        }
    }
}
