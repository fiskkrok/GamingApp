namespace GamingApp.ApiService.Validation;

public record CategoryRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? Icon { get; init; }
}