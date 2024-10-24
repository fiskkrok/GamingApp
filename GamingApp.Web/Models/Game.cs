namespace GamingApp.Web.Models;

public class Game(Category genre)
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? PictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public Category Genre { get; set; } = genre;
    public string? Developer { get; set; }
    public DateTime? LastPlayedDate { get; set; }
    public ICollection<GameSession> GameSessions { get; set; } = [];
    public ICollection<UserProfile> Players { get; set; } = [];
}