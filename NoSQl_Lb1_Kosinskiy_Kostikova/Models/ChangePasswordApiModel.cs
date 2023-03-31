using System.ComponentModel.DataAnnotations;

namespace LR1Backend.API.Models;

public class ChangePasswordApiModel
{
    [Required] public string OldPassword { get; set; } = null!;
    [Required] public string NewPassword { get; set; } = null!;
}