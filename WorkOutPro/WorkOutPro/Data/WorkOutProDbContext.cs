using Microsoft.EntityFrameworkCore;
using WorkOutPro.Models;

namespace WorkOutPro.Data;

public class WorkOutProDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ExerciseType> ExerciseTypes { get; set; }
    public DbSet<TrainingSession> TrainingSessions { get; set; }
    public DbSet<TrainingExercise> TrainingExercises { get; set; }

    public WorkOutProDbContext()
    {
        // Ensure database is created
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "workoutpro.db");
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }
}
