using System.ComponentModel.DataAnnotations;

namespace WorkOutPro.Models;

public class TrainingExercise
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Exercise type must be selected")]
    public int ExerciseTypeId { get; set; }
    public ExerciseType? ExerciseType { get; set; }

    [Required(ErrorMessage = "Training session must be selected")]
    public int TrainingSessionId { get; set; }
    public TrainingSession? TrainingSession { get; set; }

    [Required(ErrorMessage = "Load value is required")]
    [Range(0, 500, ErrorMessage = "Load must be between 0-500 kg")]
    public int Load { get; set; }      // kg

    [Required(ErrorMessage = "Number of sets is required")]
    [Range(1, 20, ErrorMessage = "Sets must be between 1-20")]
    public int Sets { get; set; }

    [Required(ErrorMessage = "Number of repetitions is required")]
    [Range(1, 100, ErrorMessage = "Repetitions must be between 1-100")]
    public int Repetitions { get; set; }

    // User relationship
    public int UserId { get; set; }
    public User? User { get; set; }
}
