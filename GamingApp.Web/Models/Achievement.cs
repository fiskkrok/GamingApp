namespace GamingApp.Web.Models;

public class Achievement
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Icon { get; set; }
    public int Score { get; set; }
    public DateTime UnlockedDate { get; set; }
    public ICollection<UserProfile> UnlockedBy { get; set; } = (List<UserProfile>) [];
}