using System.Text.RegularExpressions;
using FluentValidation;
using Ganss.Xss;

namespace GamingApp.ApiService.Validation;

public class CategoryValidator : AbstractValidator<CategoryRequest>
{
    private static readonly HtmlSanitizer Sanitizer = new();

    public CategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .Must(name => BeValidCategoryName(Sanitizer.Sanitize(name)))
            .WithMessage("Category name contains invalid characters");

        RuleFor(x => x.Description)
            .Must(description => description == null || description.Length <= 200)
            .WithMessage("Description must be 200 characters or less")
            .When(x => x.Description != null);

        RuleFor(x => x.Icon)
            .Must(icon => icon == null || icon.Length <= 200)
            .WithMessage("Icon must be 200 characters or less")
            .When(x => x.Icon != null);
    }

    private static bool BeValidCategoryName(string name)
    {
        return !Regex.IsMatch(name, @"[<>()\/\\&%#@!]");
    }
}
