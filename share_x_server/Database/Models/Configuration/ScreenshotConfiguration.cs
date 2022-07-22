using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShareXServer.Database.Models.Configuration;

public class ScreenshotConfiguration : IEntityTypeConfiguration<Screenshot>
{
    public void Configure(EntityTypeBuilder<Screenshot> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.UploadedAt).IsRequired();
        entity.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        entity.Property(x => x.DeleteToken).HasMaxLength(255).IsRequired();

        entity.HasIndex(x => x.DeleteToken).IsUnique();
    }
}