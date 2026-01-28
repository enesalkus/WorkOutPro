using System.ComponentModel.DataAnnotations;

namespace WorkOutPro.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "User"; // "Admin" or "User"

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Helper property
    public string FullName => $"{FirstName} {LastName}";
}
