using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace GamingApp.ApiService.Data.Models;

public class Game
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public string? PictureUrl { get; init; }
    public DateTime CreatedAt { get; init; }
    public int GenreId { get; init; }
    public Category? Genre { get; init; }
    public string? Developer { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.CircularDependency
    public ICollection<GameSession> GameSessions { get; init; } = [];

    // Rename this to make the relationship clearer
    // ReSharper disable once EntityFramework.ModelValidation.CircularDependency
    public ICollection<User> Players { get; init; } = [];
}