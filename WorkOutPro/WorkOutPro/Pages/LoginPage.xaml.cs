using WorkOutPro.Services;

namespace WorkOutPro.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        var (success, message) = await AuthService.Instance.LoginAsync(email ?? "", password ?? "");

        if (success)
        {
            await DisplayAlert("Success", message, "OK");

            // Navigate to main app using TabbedPage
            if (Application.Current?.Windows.Count > 0)
            {
                Application.Current.Windows[0].Page = new MainTabbedPage();
            }
        }
        else
        {
            await DisplayAlert("Error", message, "OK");
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}
