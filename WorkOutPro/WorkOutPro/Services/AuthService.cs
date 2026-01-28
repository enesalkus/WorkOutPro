using WorkOutPro.Data;
using WorkOutPro.Models;
using System.Security.Cryptography;
using System.Text;

namespace WorkOutPro.Services;

public class AuthService
{
    private static AuthService? _instance;
    public static AuthService Instance => _instance ??= new AuthService();

    private User? _currentUser;

    public User? CurrentUser => _currentUser;
    public bool IsLoggedIn => _currentUser != null;
    public bool IsAdmin => _currentUser?.Role == "Admin";

    public event Action? AuthStateChanged;

    private AuthService() { }

    public async Task<(bool success, string message)> RegisterAsync(string firstName, string lastName, string email, string password, string role = "User")
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            return (false, "First name and last name are required.");

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return (false, "Email and password are required.");

        if (password.Length < 6)
            return (false, "Password must be at least 6 characters.");

        using var db = new WorkOutProDbContext();

        // Check if email already exists
        if (db.Users.Any(u => u.Email.ToLower() == email.ToLower()))
            return (false, "This email is already registered.");

        var user = new User
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.ToLower().Trim(),
            PasswordHash = HashPassword(password),
            Role = role,
            CreatedAt = DateTime.Now
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        _currentUser = user;
        AuthStateChanged?.Invoke();

        return (true, "Registration successful!");
    }

    public async Task<(bool success, string message)> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return (false, "Email and password are required.");

        using var db = new WorkOutProDbContext();

        var user = db.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower().Trim());

        if (user == null)
            return (false, "User not found.");

        if (user.PasswordHash != HashPassword(password))
            return (false, "Incorrect password.");

        _currentUser = user;
        AuthStateChanged?.Invoke();

        return (true, $"Welcome, {user.FirstName}!");
    }

    public void Logout()
    {
        _currentUser = null;
        AuthStateChanged?.Invoke();
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    // Seed default admin user
    public async Task SeedDefaultUsersAsync()
    {
        using var db = new WorkOutProDbContext();

        if (!db.Users.Any())
        {
            // Create default admin
            db.Users.Add(new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@workoutpro.local",
                PasswordHash = HashPassword("Admin123!"),
                Role = "Admin",
                CreatedAt = DateTime.Now
            });

            // Create default user
            db.Users.Add(new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "user1@workoutpro.local",
                PasswordHash = HashPassword("User123!"),
                Role = "User",
                CreatedAt = DateTime.Now
            });

            await db.SaveChangesAsync();
        }
    }

    // --- New Management Methods ---

    public async Task<(bool success, string message)> UpdateUserAsync(User user)
    {
        try
        {
            using var db = new WorkOutProDbContext();
            db.Users.Update(user);
            await db.SaveChangesAsync();

            // Update current user if it's the one logged in
            if (_currentUser?.Id == user.Id)
            {
                _currentUser = user;
                AuthStateChanged?.Invoke();
            }
            return (true, "Information updated.");
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> ChangePasswordAsync(int userId, string newPassword)
    {
        try
        {
            if (newPassword.Length < 6)
                return (false, "Password must be at least 6 characters.");

            using var db = new WorkOutProDbContext();
            var user = await db.Users.FindAsync(userId);
            if (user != null)
            {
                user.PasswordHash = HashPassword(newPassword);
                await db.SaveChangesAsync();
                return (true, "Password changed successfully.");
            }
            return (false, "User not found.");
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }

    public async Task DeleteUserAsync(int userId)
    {
        using var db = new WorkOutProDbContext();
        var user = await db.Users.FindAsync(userId);
        if (user != null)
        {
            // Cascade delete: Exercises & Sessions
            var exercises = db.TrainingExercises.Where(e => e.UserId == userId);
            db.TrainingExercises.RemoveRange(exercises);

            var sessions = db.TrainingSessions.Where(s => s.UserId == userId);
            db.TrainingSessions.RemoveRange(sessions);

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            // If deleting self, logout
            if (_currentUser?.Id == userId)
            {
                Logout();
            }
        }
    }

    public List<User> GetAllUsers()
    {
        using var db = new WorkOutProDbContext();
        return db.Users.ToList();
    }
}
