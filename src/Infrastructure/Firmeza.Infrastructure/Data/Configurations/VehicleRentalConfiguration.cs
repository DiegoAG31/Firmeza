using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for VehicleRental
/// </summary>
public class VehicleRentalConfiguration : IEntityTypeConfiguration<VehicleRental>
{
    public void Configure(EntityTypeBuilder<VehicleRental> builder)
    {
        builder.ToTable("VehicleRentals");

        builder.HasKey(vr => vr.Id);

        builder.Property(vr => vr.RentalNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(vr => vr.TotalAmount)
            .HasPrecision(18, 2);

        // Indexes
        builder.HasIndex(vr => vr.RentalNumber)
            .IsUnique();

        builder.HasIndex(vr => vr.VehicleId);

        builder.HasIndex(vr => vr.CustomerId);

        builder.HasIndex(vr => vr.StartDate);

        // Relationships
        builder.HasOne(vr => vr.Vehicle)
            .WithMany(v => v.Rentals)
            .HasForeignKey(vr => vr.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(vr => vr.Customer)
            .WithMany(c => c.Rentals)
            .HasForeignKey(vr => vr.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
