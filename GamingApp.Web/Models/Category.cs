namespace GamingApp.Web.Models;

public class Category
{
    public int Id { get; set; }
    public int GameCount { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public ICollection<Game> Games { get; set; } = (List<Game>) [];
}