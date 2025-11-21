using System.ComponentModel.DataAnnotations;

namespace Firmeza.Web.Mvc.Models;

public class CreateSaleViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un cliente")]
    public int CustomerId { get; set; }

    public List<SaleDetailViewModel> Details { get; set; } = new List<SaleDetailViewModel>();
}

public class SaleDetailViewModel
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Quantity { get; set; }
}
