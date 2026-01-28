using WorkOutPro.Data;
using WorkOutPro.Pages;
using WorkOutPro.Services;

namespace WorkOutPro;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        UpdateUserInfo();
        LoadStats();
    }

    private void UpdateUserInfo()
    {
        var user = AuthService.Instance.CurrentUser;
        if (user != null)
        {
            UserEmailLabel.Text = user.Email;

            // Format: Admin için "👑 Admin", User için "👤 İsim Soyisim (User)"
            if (user.Role == "Admin")
            {
                UserRoleLabel.Text = "👑 Admin";
            }
            else
            {
                UserRoleLabel.Text = $"👤 {user.FullName} (User)";
            }
        }
    }

    private void LoadStats()
    {
        try
        {
            var userId = AuthService.Instance.CurrentUser?.Id ?? 0;
            using var db = new WorkOutProDbContext();

            // User-specific counts
            SessionCountLabel.Text = db.TrainingSessions.Count(s => s.UserId == userId).ToString();
            ExerciseCountLabel.Text = db.TrainingExercises.Count(e => e.UserId == userId).ToString();
        }
        catch
        {
            SessionCountLabel.Text = "0";
            ExerciseCountLabel.Text = "0";
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (confirm)
        {
            AuthService.Instance.Logout();

            // Navigate to login
            if (Application.Current?.Windows.Count > 0)
            {
                Application.Current.Windows[0].Page = new NavigationPage(new LoginPage());
            }
        }
    }
}
