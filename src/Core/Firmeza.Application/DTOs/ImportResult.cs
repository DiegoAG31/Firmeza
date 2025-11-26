namespace Firmeza.Application.DTOs;

public class ImportResult
{
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}
