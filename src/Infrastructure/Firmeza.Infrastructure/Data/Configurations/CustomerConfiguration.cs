using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Customer
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.DocumentType)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(c => c.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Address)
            .HasMaxLength(300);

        builder.Property(c => c.City)
            .HasMaxLength(100);

        builder.Property(c => c.UserId)
            .HasMaxLength(450); // Standard for Identity UserId

        // Indexes
        builder.HasIndex(c => c.DocumentNumber)
            .IsUnique();

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.HasIndex(c => c.UserId);

        // Computed column (not persisted, just for queries)
        builder.Ignore(c => c.FullName);
    }
}
