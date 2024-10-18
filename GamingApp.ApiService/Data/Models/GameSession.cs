namespace GamingApp.ApiService.Data.Models;

public class GameSession
{
    public int Id { get; init; } // Primary Key
    public int UserId { get; init; } // Foreign Key
    public int GameId { get; init; } // Foreign Key
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int Score { get; init; }

    // Navigation properties
    public required User? User { get; init; }
    public required Game? Game { get; init; }
}