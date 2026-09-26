using System.ComponentModel.DataAnnotations;

namespace Microservice.Web.Attributes;

public class ValidateImageFileAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions = { ".png", ".jpg", ".jpeg", ".gif" };

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        if (value is not IFormFile file)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!_allowedExtensions.Contains(extension))
        {
            ErrorMessage = $"The Image field only accepts files with the following extensions: {string.Join(", ", _allowedExtensions)}";
            return false;
        }

        return true;
    }
}
