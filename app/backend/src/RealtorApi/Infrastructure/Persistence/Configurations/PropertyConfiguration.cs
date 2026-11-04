using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtorApi.Domain.Properties;

namespace RealtorApi.Infrastructure.Persistence.Configurations;

public sealed class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("properties", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_properties_bedroom_count", "\"BedroomCount\" >= 0");
            tableBuilder.HasCheckConstraint("CK_properties_bathroom_count", "\"BathroomCount\" >= 0");
            tableBuilder.HasCheckConstraint("CK_properties_area_square_meters", "\"AreaSquareMeters\" > 0");
        });

        builder.HasKey(property => property.Id);

        builder.Property(property => property.Title)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(property => property.Description)
            .HasMaxLength(1200)
            .IsRequired();

        builder.Property(property => property.Address)
            .HasMaxLength(240)
            .IsRequired();

        builder.Property(property => property.Price)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(property => property.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(property => property.BedroomCount)
            .IsRequired();

        builder.Property(property => property.BathroomCount)
            .IsRequired();

        builder.Property(property => property.AreaSquareMeters)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(property => property.ImageUrl)
            .HasMaxLength(260)
            .IsRequired(false);

        builder.Property(property => property.CreatedAt)
            .IsRequired();

        builder.Property(property => property.UpdatedAt);

        builder.HasIndex(property => property.Status);
    }
}
