using WorkOutPro.Data;
using WorkOutPro.Models;
using WorkOutPro.Services;

namespace WorkOutPro.Pages;

public partial class ExerciseTypesPage : ContentPage
{
    private readonly WorkOutProDbContext _db;

    public ExerciseTypesPage()
    {
        InitializeComponent();
        _db = new WorkOutProDbContext();
        LoadData();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateAdminVisibility();
        LoadData();
    }

    private void UpdateAdminVisibility()
    {
        bool isAdmin = AuthService.Instance.IsAdmin;
        AddFrame.IsVisible = isAdmin;
        NotAdminFrame.IsVisible = !isAdmin;
    }

    private void LoadData()
    {
        ExerciseList.ItemsSource = _db.ExerciseTypes.ToList();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (!AuthService.Instance.IsAdmin)
        {
            await DisplayAlert("Unauthorized", "Only Admin users can add exercise types.", "OK");
            return;
        }

        var name = NewExerciseName.Text?.Trim();
        if (string.IsNullOrEmpty(name)) return;

        _db.ExerciseTypes.Add(new ExerciseType { Name = name });
        await _db.SaveChangesAsync();
        NewExerciseName.Text = string.Empty;
        LoadData();
    }

    private async void OnDetailClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ExerciseType exercise)
        {
            await DisplayAlert("Exercise Details",
                $"Name: {exercise.Name}\nID: {exercise.Id}",
                "OK");
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (!AuthService.Instance.IsAdmin)
        {
            await DisplayAlert("Unauthorized", "Only Admin users can edit.", "OK");
            return;
        }

        if (sender is Button btn && btn.CommandParameter is ExerciseType exercise)
        {
            string result = await DisplayPromptAsync("Edit",
                "Enter new name:",
                initialValue: exercise.Name,
                keyboard: Keyboard.Text);

            if (!string.IsNullOrEmpty(result) && result != exercise.Name)
            {
                exercise.Name = result;
                await _db.SaveChangesAsync();
                LoadData();
            }
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (!AuthService.Instance.IsAdmin)
        {
            await DisplayAlert("Unauthorized", "Only Admin users can delete.", "OK");
            return;
        }

        if (sender is Button btn && btn.CommandParameter is ExerciseType exercise)
        {
            bool confirm = await DisplayAlert("Delete", $"Delete '{exercise.Name}'?", "Yes", "No");
            if (confirm)
            {
                _db.ExerciseTypes.Remove(exercise);
                await _db.SaveChangesAsync();
                LoadData();
            }
        }
    }
}
