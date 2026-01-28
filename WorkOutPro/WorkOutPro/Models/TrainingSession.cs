using System.ComponentModel.DataAnnotations;

namespace WorkOutPro.Models;

public class TrainingSession
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    public DateTime EndTime { get; set; }

    // User relationship
    public int UserId { get; set; }
    public User? User { get; set; }
}
