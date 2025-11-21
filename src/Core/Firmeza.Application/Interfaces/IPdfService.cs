using Firmeza.Domain.Entities;

namespace Firmeza.Application.Interfaces;

public interface IPdfService
{
    byte[] GenerateSaleReceipt(Sale sale);
}
