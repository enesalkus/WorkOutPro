using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WorkOutPro.Data;
using WorkOutPro.Models;
using Microsoft.EntityFrameworkCore;

namespace WorkOutPro.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly WorkOutProDbContext _db;

    [ObservableProperty]
    private ObservableCollection<ExerciseType> exerciseTypes = new();

    [ObservableProperty]
    private ObservableCollection<TrainingSession> trainingSessions = new();

    [ObservableProperty]
    private ObservableCollection<TrainingExercise> trainingExercises = new();

    [ObservableProperty]
    private string newExerciseName = string.Empty;

    public MainViewModel(WorkOutProDbContext db)
    {
        _db = db;
        LoadData();
    }

    private void LoadData()
    {
        var types = _db.ExerciseTypes.ToList();
        ExerciseTypes = new ObservableCollection<ExerciseType>(types);

        var sessions = _db.TrainingSessions.OrderByDescending(s => s.StartTime).ToList();
        TrainingSessions = new ObservableCollection<TrainingSession>(sessions);

        var exercises = _db.TrainingExercises.Include(e => e.ExerciseType).Include(e => e.TrainingSession).ToList();
        TrainingExercises = new ObservableCollection<TrainingExercise>(exercises);
    }

    [RelayCommand]
    private void AddExerciseType()
    {
        if (string.IsNullOrWhiteSpace(NewExerciseName)) return;

        var newType = new ExerciseType { Name = NewExerciseName };
        _db.ExerciseTypes.Add(newType);
        _db.SaveChanges();
        ExerciseTypes.Add(newType);
        NewExerciseName = string.Empty;
    }

    [RelayCommand]
    private void DeleteExerciseType(ExerciseType exerciseType)
    {
        _db.ExerciseTypes.Remove(exerciseType);
        _db.SaveChanges();
        ExerciseTypes.Remove(exerciseType);
    }

    [RelayCommand]
    private void AddSession()
    {
        var session = new TrainingSession
        {
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(1)
        };
        _db.TrainingSessions.Add(session);
        _db.SaveChanges();
        TrainingSessions.Insert(0, session);
    }

    [RelayCommand]
    private void DeleteSession(TrainingSession session)
    {
        _db.TrainingSessions.Remove(session);
        _db.SaveChanges();
        TrainingSessions.Remove(session);
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadData();
    }
}
