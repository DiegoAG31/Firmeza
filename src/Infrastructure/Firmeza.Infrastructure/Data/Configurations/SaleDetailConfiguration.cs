using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for SaleDetail
/// </summary>
public class SaleDetailConfiguration : IEntityTypeConfiguration<SaleDetail>
{
    public void Configure(EntityTypeBuilder<SaleDetail> builder)
    {
        builder.ToTable("SaleDetails");

        builder.HasKey(sd => sd.Id);

        builder.Property(sd => sd.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(sd => sd.Subtotal)
            .HasPrecision(18, 2);

        // Indexes
        builder.HasIndex(sd => sd.SaleId);

        builder.HasIndex(sd => sd.ProductId);

        // Relationships
        builder.HasOne(sd => sd.Product)
            .WithMany(p => p.SaleDetails)
            .HasForeignKey(sd => sd.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
