using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Vehicle
/// </summary>
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Brand)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.PlateNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(v => v.PricePerDay)
            .HasPrecision(18, 2);

        builder.Property(v => v.ImageUrl)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(v => v.PlateNumber)
            .IsUnique();

        builder.HasIndex(v => v.Status);
    }
}
