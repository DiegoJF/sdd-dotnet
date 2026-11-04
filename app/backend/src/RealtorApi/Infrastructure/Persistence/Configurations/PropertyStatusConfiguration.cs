using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtorApi.Domain.Properties;

namespace RealtorApi.Infrastructure.Persistence.Configurations;

public sealed class PropertyStatusConfiguration : IEntityTypeConfiguration<PropertyStatus>
{
    public void Configure(EntityTypeBuilder<PropertyStatus> builder)
    {
        builder.ToTable("property_statuses");

        builder.HasKey(status => status.Value);

        builder.Property(status => status.Value)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(status => status.Description)
            .HasMaxLength(200)
            .IsRequired();
    }
}
