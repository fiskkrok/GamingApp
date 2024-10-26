using System.Globalization;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GamingApp.ApiService.Data.Models.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.IdentityServerSid)
            .IsUnique();

        builder.HasIndex(x => x.InGameUserName)
            .IsUnique();

        builder.HasIndex(x => x.Email);

        builder.Property(x => x.IdentityServerSid)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false); // Since it's always ASCII


        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(50);
            //.UseCollation("default"); // Case-insensitive comparison

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(100);
            //.UseCollation("default");

        builder.Property(x => x.InGameUserName)
            .IsRequired()
            .HasMaxLength(50);
            //.UseCollation("default");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.FavoriteGame)
            .HasMaxLength(100);

        builder.Property(x => x.Bio)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasMaxLength(50);

        // Add check constraints
        //builder.ToTable(tb => tb.HasCheckConstraint(
        //    "CK_Users_InGameUserName_Format",
        //    "InGameUserName REGEXP '^[a-zA-Z0-9_-]*$'"
        //));

        //builder.ToTable(tb => tb.HasCheckConstraint(
        //    "CK_Users_Email_Format",
        //    "Email REGEXP '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$'"
        //));

        // Relationships
        builder.HasMany(x => x.GameSessions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior
                .Cascade); // Delete all game sessions when a user is deleted

    }
}
