using WorkOutPro.Services;

namespace WorkOutPro.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var firstName = FirstNameEntry.Text?.Trim();
        var lastName = LastNameEntry.Text?.Trim();
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;
        var confirmPassword = ConfirmPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(firstName))
        {
            await DisplayAlert("Error", "First name is required.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            await DisplayAlert("Error", "Last name is required.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Error", "Email is required.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Password is required.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        // All new users are "User" type
        var (success, message) = await AuthService.Instance.RegisterAsync(firstName, lastName, email, password, "User");

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

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
