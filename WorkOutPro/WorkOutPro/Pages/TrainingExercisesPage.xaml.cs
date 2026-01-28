using WorkOutPro.Data;
using WorkOutPro.Models;
using WorkOutPro.Services;
using Microsoft.EntityFrameworkCore;

namespace WorkOutPro.Pages;

public partial class TrainingExercisesPage : ContentPage
{
    private readonly WorkOutProDbContext _db;

    public TrainingExercisesPage()
    {
        InitializeComponent();
        _db = new WorkOutProDbContext();
        LoadPickers();
        LoadData();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadPickers();
        LoadData();
    }

    private void LoadPickers()
    {
        var userId = AuthService.Instance.CurrentUser?.Id ?? 0;

        ExerciseTypePicker.ItemsSource = _db.ExerciseTypes.ToList();

        // Only show user's sessions with time range
        SessionPicker.ItemsSource = _db.TrainingSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartTime)
            .ToList()
            .Select(s => new SessionDisplayItem
            {
                Id = s.Id,
                DisplayText = $"{s.StartTime:dd.MM.yyyy} ({s.StartTime:HH:mm} - {s.EndTime:HH:mm})"
            })
            .ToList();
    }

    private void LoadData()
    {
        var userId = AuthService.Instance.CurrentUser?.Id ?? 0;

        var exercises = _db.TrainingExercises
            .Include(e => e.ExerciseType)
            .Include(e => e.TrainingSession)
            .Where(e => e.UserId == userId)
            .ToList()
            .Select(e => new ExerciseDisplayItem
            {
                Id = e.Id,
                ExerciseTypeName = e.ExerciseType?.Name ?? "Unknown",
                SessionDate = e.TrainingSession != null
                    ? $"{e.TrainingSession.StartTime:dd.MM.yyyy} ({e.TrainingSession.StartTime:HH:mm} - {e.TrainingSession.EndTime:HH:mm})"
                    : "Unknown",
                Load = e.Load,
                Sets = e.Sets,
                Repetitions = e.Repetitions,
                LoadText = $"{e.Load} kg",
                SetsText = $"{e.Sets} set",
                RepsText = $"{e.Repetitions} reps"
            })
            .ToList();

        ExercisesList.ItemsSource = exercises;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var userId = AuthService.Instance.CurrentUser?.Id ?? 0;
        if (userId == 0)
        {
            await DisplayAlert("Error", "Please log in.", "OK");
            return;
        }

        if (ExerciseTypePicker.SelectedItem is not ExerciseType selectedType)
        {
            await DisplayAlert("Warning", "Please select an exercise type.", "OK");
            return;
        }

        if (SessionPicker.SelectedItem is not SessionDisplayItem selectedSession)
        {
            await DisplayAlert("Warning", "Please select a training session.", "OK");
            return;
        }

        if (!int.TryParse(LoadEntry.Text, out int load) || load <= 0)
        {
            await DisplayAlert("Warning", "Please enter a valid load.", "OK");
            return;
        }

        if (!int.TryParse(SetsEntry.Text, out int sets) || sets <= 0)
        {
            await DisplayAlert("Warning", "Please enter a valid number of sets.", "OK");
            return;
        }

        if (!int.TryParse(RepsEntry.Text, out int reps) || reps <= 0)
        {
            await DisplayAlert("Warning", "Please enter a valid number of reps.", "OK");
            return;
        }

        // Check if exercise already exists in this session
        bool exists = _db.TrainingExercises.Any(e =>
            e.UserId == userId &&
            e.TrainingSessionId == selectedSession.Id &&
            e.ExerciseTypeId == selectedType.Id);

        if (exists)
        {
            await DisplayAlert("Error", "This exercise already exists in this session. Please select a different exercise.", "OK");
            return;
        }

        var exercise = new TrainingExercise
        {
            ExerciseTypeId = selectedType.Id,
            TrainingSessionId = selectedSession.Id,
            Load = load,
            Sets = sets,
            Repetitions = reps,
            UserId = userId
        };

        _db.TrainingExercises.Add(exercise);
        await _db.SaveChangesAsync();

        // Clear form
        LoadEntry.Text = string.Empty;
        SetsEntry.Text = string.Empty;
        RepsEntry.Text = string.Empty;

        LoadData();
        await DisplayAlert("Success", "Exercise added!", "OK");
    }

    private async void OnDetailClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ExerciseDisplayItem exercise)
        {
            await DisplayAlert("Exercise Details",
                $"Type: {exercise.ExerciseTypeName}\n" +
                $"Session: {exercise.SessionDate}\n" +
                $"Load: {exercise.Load} kg\n" +
                $"Sets: {exercise.Sets}\n" +
                $"Reps: {exercise.Repetitions}\n" +
                $"Total Reps: {exercise.Sets * exercise.Repetitions}",
                "OK");
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ExerciseDisplayItem displayItem)
        {
            var exercise = await _db.TrainingExercises.FindAsync(displayItem.Id);
            if (exercise == null) return;

            string loadResult = await DisplayPromptAsync("Edit - Load",
                "New load (kg):",
                initialValue: exercise.Load.ToString(),
                keyboard: Keyboard.Numeric);

            if (!string.IsNullOrEmpty(loadResult) && int.TryParse(loadResult, out int newLoad))
            {
                exercise.Load = newLoad;
            }

            string setsResult = await DisplayPromptAsync("Edit - Sets",
                "New number of sets:",
                initialValue: exercise.Sets.ToString(),
                keyboard: Keyboard.Numeric);

            if (!string.IsNullOrEmpty(setsResult) && int.TryParse(setsResult, out int newSets))
            {
                exercise.Sets = newSets;
            }

            string repsResult = await DisplayPromptAsync("Edit - Reps",
                "New number of reps:",
                initialValue: exercise.Repetitions.ToString(),
                keyboard: Keyboard.Numeric);

            if (!string.IsNullOrEmpty(repsResult) && int.TryParse(repsResult, out int newReps))
            {
                exercise.Repetitions = newReps;
            }

            await _db.SaveChangesAsync();
            LoadData();
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ExerciseDisplayItem displayItem)
        {
            bool confirm = await DisplayAlert("Delete", "Delete this exercise?", "Yes", "No");
            if (confirm)
            {
                var exercise = await _db.TrainingExercises.FindAsync(displayItem.Id);
                if (exercise != null)
                {
                    _db.TrainingExercises.Remove(exercise);
                    await _db.SaveChangesAsync();
                    LoadData();
                }
            }
        }
    }
}

public class SessionDisplayItem
{
    public int Id { get; set; }
    public string DisplayText { get; set; } = string.Empty;
}

public class ExerciseDisplayItem
{
    public int Id { get; set; }
    public string ExerciseTypeName { get; set; } = string.Empty;
    public string SessionDate { get; set; } = string.Empty;
    public int Load { get; set; }
    public int Sets { get; set; }
    public int Repetitions { get; set; }
    public string LoadText { get; set; } = string.Empty;
    public string SetsText { get; set; } = string.Empty;
    public string RepsText { get; set; } = string.Empty;
}
