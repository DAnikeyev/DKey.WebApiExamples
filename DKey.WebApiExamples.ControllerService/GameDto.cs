using System.ComponentModel.DataAnnotations;
using DKey.WebApiExamples.ControllerService.Attributes;

namespace DKey.WebApiExamples.ControllerService;

public record GameDto
{
    [Required]
    [NoMinecraft]
    public string Name { get; init; } = null!;

    public int Score { get; init; }

    public string? Comment { get; init; }
}