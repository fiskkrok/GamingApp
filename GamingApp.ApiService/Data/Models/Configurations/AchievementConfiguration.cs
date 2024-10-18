using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GamingApp.ApiService.Data.Models.Configurations;

public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.ToTable("Achievements");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.Score)
            .IsRequired();
        builder.Property(x => x.Icon)
            .HasMaxLength(200);
        builder.Property(x => x.UnlockedDate)
            .IsRequired();

        // Add this to configure the many-to-many relationship
        builder.HasMany(x => x.UnlockedBy)
            .WithMany(x => x.AchievementsUnlocked)
            .UsingEntity(j => j.ToTable("UserAchievements"));
    }
}