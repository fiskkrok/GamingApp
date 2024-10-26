namespace GamingApp.ApiService.Validation;

public record CreateUserProfileRequest
{
    public required string InGameUserName { get; init; }
    public string? Bio { get; init; }
    public string? Status { get; init; }
}