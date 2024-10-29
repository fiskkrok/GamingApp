using System.Text.RegularExpressions;
using FluentValidation;
using Ganss.Xss;

namespace GamingApp.ApiService.Validation;

public class CreateUserProfileValidator : AbstractValidator<CreateUserProfileRequest>
{
    private static readonly HtmlSanitizer Sanitizer = new();

    public CreateUserProfileValidator()
    {
        RuleFor(x => x.InGameUserName)
            .NotEmpty()
            .Length(3, 50)
            .Matches("^[a-zA-Z0-9_-]*$")
            .WithMessage("Username can only contain letters, numbers, underscores, and hyphens")
            .Must(BeValidUsername)
            .WithMessage("Username contains invalid characters or format");

        RuleFor(x => x.Bio)
            .Must((request, bio) => bio == null || bio == Sanitizer.Sanitize(bio))
            .MaximumLength(500)
            .When(x => x.Bio != null);

        RuleFor(x => x.Status)
            .Must((request, status) => status == null || status == Sanitizer.Sanitize(status))
            .MaximumLength(50)
            .When(x => x.Status != null);
    }

    private static bool BeValidUsername(string username)
    {
        // Additional validation to prevent common injection patterns
        var invalidPatterns = new[]
        {
            @"<[^>]*>",           // HTML tags
            @"javascript:",       // JavaScript protocol
            @"data:",            // Data URI scheme
            @"vbscript:",        // VBScript protocol
            @"onload=",          // Event handlers
            @"onerror=",
            @"onclick=",
            @"eval\(",           // JavaScript eval
            @"document\.",       // DOM manipulation
            @"window\."          // Window object access
        };

        return invalidPatterns.All(pattern => !Regex.IsMatch(username, pattern, RegexOptions.IgnoreCase));
    }
}
