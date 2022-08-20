using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShareXServer.Database.Enums;

namespace ShareXServer.Database.Models.Configuration;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.UploadedAt).IsRequired();
        entity.Property(x => x.MediaType).HasConversion<EnumToStringConverter<MediaType>>().IsRequired();
        entity.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        entity.Property(x => x.OriginalFileName).HasMaxLength(255).IsRequired();
        entity.Property(x => x.DeleteToken).HasMaxLength(255).IsRequired();
        entity.Property(x => x.MimeType).HasMaxLength(255).IsRequired();

        entity.HasIndex(x => x.DeleteToken).IsUnique();
    }
}