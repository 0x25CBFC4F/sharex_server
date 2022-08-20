using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShareXServer.Database.Models.Configuration;

public class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
{
    public void Configure(EntityTypeBuilder<ShortenedUrl> entity)
    {
        entity.HasKey(x => x.Id);
        
        entity.Property(x => x.RealUrl).HasMaxLength(512).IsRequired();
    }
}