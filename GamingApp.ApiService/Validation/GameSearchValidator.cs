using FluentValidation;
using Ganss.Xss;

namespace GamingApp.ApiService.Validation;

public class GameSearchValidator : AbstractValidator<GameSearchRequest>
{
    private static readonly HtmlSanitizer Sanitizer = new();

    public GameSearchValidator()
    {
        RuleFor(x => x.SearchTerm)
            .Must(str => str == null || str == Sanitizer.Sanitize(str))
            .MaximumLength(100)
            .When(x => x.SearchTerm != null);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");
    }
}
