using System.ComponentModel.DataAnnotations;

namespace LR1Backend.API.Models;

public class EditNodeModel
{
    [Required] public string NewTitle { get; set; } = null!;
    [Required] public string NewText { get; set; } = null!;
}