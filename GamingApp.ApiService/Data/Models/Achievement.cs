namespace GamingApp.ApiService.Data.Models;

public class Achievement
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Icon { get; init; }
    public int Score { get; init; }
    public DateTime UnlockedDate { get; init; }

    // Add this line to define the inverse navigation property
    public ICollection<User> UnlockedBy { get; init; } = [];
}