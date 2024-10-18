using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GamingApp.ApiService.Data.Models.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.Description)
            .HasMaxLength(500);
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.Property(x => x.Developer)
            .HasMaxLength(50);
        builder.Property(x => x.PictureUrl)
            .HasMaxLength(200);

        builder.HasMany(x => x.GameSessions)
            .WithOne(x => x.Game)
            .HasForeignKey(x => x.GameId);

        // Add this configuration for Genre
        builder.HasOne(x => x.Genre)
            .WithMany(x => x.Games)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Players)
            .WithMany(x => x.PlayedGames)
            .UsingEntity(j => j.ToTable("UserGames"));
    }
}