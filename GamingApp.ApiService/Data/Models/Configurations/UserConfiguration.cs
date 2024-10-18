using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GamingApp.ApiService.Data.Models.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IdentityServerSid).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Username).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
        builder.Property(x => x.InGameUserName).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.FavoriteGame).HasMaxLength(100);
        builder.Property(x => x.Bio).HasMaxLength(500);
        builder.Property(x => x.Status).HasMaxLength(50);

        builder.HasMany(x => x.GameSessions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.AchievementsUnlocked)
            .WithMany(x => x.UnlockedBy)
            .UsingEntity(j => j.ToTable("UserAchievements"));

    }
}