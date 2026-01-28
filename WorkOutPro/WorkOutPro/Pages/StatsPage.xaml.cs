using WorkOutPro.Data;
using WorkOutPro.Services;
using Microsoft.EntityFrameworkCore;

namespace WorkOutPro.Pages;

public partial class StatsPage : ContentPage
{
    private readonly WorkOutProDbContext _db;

    public StatsPage()
    {
        InitializeComponent();
        _db = new WorkOutProDbContext();
        LoadData();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadData();
    }

    private void LoadData()
    {
        var userId = AuthService.Instance.CurrentUser?.Id ?? 0;
        var fourWeeksAgo = DateTime.Now.AddDays(-28);

        var stats = _db.TrainingExercises
            .Include(e => e.ExerciseType)
            .Include(e => e.TrainingSession)
            .Where(e => e.UserId == userId && e.TrainingSession != null && e.TrainingSession.StartTime >= fourWeeksAgo)
            .GroupBy(e => e.ExerciseType!.Name)
            .Select(g => new ExerciseStats
            {
                ExerciseName = g.Key,
                Count = g.Count(),
                TotalReps = g.Sum(e => e.Sets * e.Repetitions),
                AvgLoad = g.Average(e => (double)e.Load),
                MaxLoad = g.Max(e => e.Load)
            })
            .ToList();

        StatsList.ItemsSource = stats;
    }
}

public class ExerciseStats
{
    public string ExerciseName { get; set; } = string.Empty;
    public int Count { get; set; }
    public int TotalReps { get; set; }
    public double AvgLoad { get; set; }
    public int MaxLoad { get; set; }
}
