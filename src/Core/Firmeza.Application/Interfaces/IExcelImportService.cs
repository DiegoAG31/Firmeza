using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

public interface IExcelImportService
{
    Task<ImportResult> ImportDataAsync(Stream fileStream);
}
