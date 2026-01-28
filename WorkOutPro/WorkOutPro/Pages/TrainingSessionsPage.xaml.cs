using WorkOutPro.Data;
using WorkOutPro.Models;
using WorkOutPro.Services;
using System.Globalization;

namespace WorkOutPro.Pages;

public partial class TrainingSessionsPage : ContentPage
{
    private readonly WorkOutProDbContext _db;

    public TrainingSessionsPage()
    {
        InitializeComponent();
        _db = new WorkOutProDbContext();

        // Set default values
        var now = DateTime.Now;
        StartDateEntry.Text = now.ToString("dd.MM.yyyy");
        StartTimeEntry.Text = "10:00";
        EndDateEntry.Text = now.ToString("dd.MM.yyyy");
        EndTimeEntry.Text = "11:00";

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
        SessionList.ItemsSource = _db.TrainingSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartTime)
            .ToList();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var userId = AuthService.Instance.CurrentUser?.Id ?? 0;
        if (userId == 0)
        {
            await DisplayAlert("Error", "Please log in.", "OK");
            return;
        }

        // Parse start date/time
        if (!TryParseDateTime(StartDateEntry.Text, StartTimeEntry.Text, out DateTime startDateTime))
        {
            await DisplayAlert("Error", "Invalid start date/time.\nFormat: Date=DD.MM.YYYY, Time=HH:MM", "OK");
            return;
        }

        // Parse end date/time
        if (!TryParseDateTime(EndDateEntry.Text, EndTimeEntry.Text, out DateTime endDateTime))
        {
            await DisplayAlert("Error", "Invalid end date/time.\nFormat: Date=DD.MM.YYYY, Time=HH:MM", "OK");
            return;
        }

        if (endDateTime <= startDateTime)
        {
            await DisplayAlert("Error", "End time must be after start time.", "OK");
            return;
        }

        // Check if session with same start time already exists
        if (_db.TrainingSessions.Any(s => s.UserId == userId && s.StartTime == startDateTime))
        {
            await DisplayAlert("Error", "A session already exists at this date and time.", "OK");
            return;
        }

        var session = new TrainingSession
        {
            StartTime = startDateTime,
            EndTime = endDateTime,
            UserId = userId
        };
        _db.TrainingSessions.Add(session);
        await _db.SaveChangesAsync();
        LoadData();

        await DisplayAlert("Success", "New session added!", "OK");
    }

    private bool TryParseDateTime(string dateText, string timeText, out DateTime result)
    {
        result = DateTime.MinValue;

        if (string.IsNullOrWhiteSpace(dateText) || string.IsNullOrWhiteSpace(timeText))
            return false;

        // Try parsing date (dd.MM.yyyy format)
        if (!DateTime.TryParseExact(dateText.Trim(), "dd.MM.yyyy",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
        {
            // Try alternate formats
            if (!DateTime.TryParseExact(dateText.Trim(), "d.M.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return false;
            }
        }

        // Try parsing time (HH:mm format)
        var timeParts = timeText.Trim().Replace(".", ":").Split(':');
        if (timeParts.Length != 2)
            return false;

        if (!int.TryParse(timeParts[0], out int hours) || hours < 0 || hours > 23)
            return false;
        if (!int.TryParse(timeParts[1], out int minutes) || minutes < 0 || minutes > 59)
            return false;

        result = date.AddHours(hours).AddMinutes(minutes);
        return true;
    }

    private async void OnDetailClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TrainingSession session)
        {
            var duration = session.EndTime - session.StartTime;
            await DisplayAlert("Session Details",
                $"Start: {session.StartTime:dd.MM.yyyy HH:mm}\n" +
                $"End: {session.EndTime:dd.MM.yyyy HH:mm}\n" +
                $"Duration: {duration.TotalMinutes:F0} minutes\n" +
                $"ID: {session.Id}",
                "OK");
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TrainingSession session)
        {
            // Get new start time
            string newStartDate = await DisplayPromptAsync("Start Date",
                "New start date (DD.MM.YYYY):",
                initialValue: session.StartTime.ToString("dd.MM.yyyy"));
            if (newStartDate == null) return;

            string newStartTime = await DisplayPromptAsync("Start Time",
                "New start time (HH:MM):",
                initialValue: session.StartTime.ToString("HH:mm"));
            if (newStartTime == null) return;

            // Get new end time
            string newEndDate = await DisplayPromptAsync("End Date",
                "New end date (DD.MM.YYYY):",
                initialValue: session.EndTime.ToString("dd.MM.yyyy"));
            if (newEndDate == null) return;

            string newEndTime = await DisplayPromptAsync("End Time",
                "New end time (HH:MM):",
                initialValue: session.EndTime.ToString("HH:mm"));
            if (newEndTime == null) return;

            // Parse and validate
            if (!TryParseDateTime(newStartDate, newStartTime, out DateTime startDateTime))
            {
                await DisplayAlert("Error", "Invalid start date/time.", "OK");
                return;
            }

            if (!TryParseDateTime(newEndDate, newEndTime, out DateTime endDateTime))
            {
                await DisplayAlert("Error", "Invalid end date/time.", "OK");
                return;
            }

            if (endDateTime <= startDateTime)
            {
                await DisplayAlert("Error", "End time must be after start time.", "OK");
                return;
            }

            session.StartTime = startDateTime;
            session.EndTime = endDateTime;
            await _db.SaveChangesAsync();
            LoadData();

            await DisplayAlert("Success", "Session updated!", "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TrainingSession session)
        {
            bool confirm = await DisplayAlert("Delete", "Delete this session?", "Yes", "No");
            if (confirm)
            {
                _db.TrainingSessions.Remove(session);
                await _db.SaveChangesAsync();
                LoadData();
            }
        }
    }
}
