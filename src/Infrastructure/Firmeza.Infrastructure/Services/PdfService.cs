using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmeza.Infrastructure.Services;

public class PdfService : IPdfService
{
    public PdfService()
    {
        // Configure license (Community)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateSaleReceipt(Sale sale)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text($"Recibo de Venta #{sale.SaleNumber}")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Item().Text($"Fecha: {sale.SaleDate:dd/MM/yyyy HH:mm}");
                        x.Item().Text($"Cliente: {sale.Customer.FullName}");
                        x.Item().Text($"Documento: {sale.Customer.DocumentNumber}");
                        
                        x.Item().PaddingVertical(0.5f, Unit.Centimetre).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Producto");
                                header.Cell().Element(CellStyle).AlignRight().Text("Precio Unit.");
                                header.Cell().Element(CellStyle).AlignRight().Text("Cant.");
                                header.Cell().Element(CellStyle).AlignRight().Text("Total");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            foreach (var item in sale.SaleDetails)
                            {
                                table.Cell().Element(CellStyle).Text(item.Product.Name);
                                table.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitPrice:C0}");
                                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity}");
                                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Subtotal:C0}");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Medium).PaddingVertical(5);
                                }
                            }
                        });

                        x.Item().PaddingTop(1, Unit.Centimetre).AlignRight().Text($"Subtotal: {sale.Subtotal:C0}");
                        x.Item().AlignRight().Text($"IVA (19%): {sale.Tax:C0}");
                        x.Item().AlignRight().Text($"TOTAL: {sale.Total:C0}").SemiBold().FontSize(14);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
            });
        })
        .GeneratePdf();
    }
}
