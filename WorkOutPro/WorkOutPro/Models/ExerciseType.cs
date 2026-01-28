using System.ComponentModel.DataAnnotations;

namespace WorkOutPro.Models;

public class ExerciseType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Exercise name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Exercise name must be between 2-100 characters")]
    public string Name { get; set; } = string.Empty;
}
