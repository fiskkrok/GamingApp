using System.ComponentModel.DataAnnotations;

namespace GamingApp.Web.Components.Dialogs;

public class CreateProfileModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [RegularExpression("^[a-zA-Z0-9_-]*$",
        ErrorMessage = "Username can only contain letters, numbers, underscores, and hyphens")]
    public string InGameUserName { get; set; } = string.Empty;
}
