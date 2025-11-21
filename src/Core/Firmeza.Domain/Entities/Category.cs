using Firmeza.Domain.Common;

namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents a category for organizing products
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Category name (e.g., Cement, Steel, Tools)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the category
    /// </summary>
    public string? Description { get; set; }

    // Navigation properties
    /// <summary>
    /// Products belonging to this category
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
