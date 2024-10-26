namespace GamingApp.ApiService.Validation;

public record GameSearchRequest
{
    public string? SearchTerm { get; init; }
    public int? CategoryId { get; init; }
    public int PageSize { get; init; }
    public int PageNumber { get; init; }
}