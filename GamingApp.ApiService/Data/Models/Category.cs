using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GamingApp.ApiService.Data.Models;

public class Category
{
    public int Id { get; init; }
    public int GameCount { get; init; }
    public required string Name { get; init; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Icon { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [JsonIgnore] // Add this attribute to ignore during serialization
    public ICollection<Game> Games { get; init; } = [];
}
