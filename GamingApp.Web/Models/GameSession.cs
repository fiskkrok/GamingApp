namespace GamingApp.Web.Models;

public class GameSession
{
    public int Id { get; set; } // Primary Key
    public int UserId { get; set; } // Foreign Key
    public int GameId { get; set; } // Foreign Key
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Score { get; set; }

    // Navigation properties
    public required UserProfile User { get; set; }
    public required Game Game { get; set; }
}