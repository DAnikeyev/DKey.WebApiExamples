using System.ComponentModel.DataAnnotations;

namespace DKey.WebApiExamples.ControllerService.Attributes;

public class NoMinecraftAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string name && name.Equals("Minecraft", StringComparison.OrdinalIgnoreCase))
        {
            return new ValidationResult("Minecraft is not allowed as a game name");
        }

        return ValidationResult.Success;
    }
}