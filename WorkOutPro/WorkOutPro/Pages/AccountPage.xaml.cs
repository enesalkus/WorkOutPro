using WorkOutPro.Services;
using WorkOutPro.Models;

namespace WorkOutPro.Pages;

public partial class AccountPage : ContentPage
{
    public AccountPage()
    {
        InitializeComponent();
        LoadUserInfo();
    }

    private void LoadUserInfo()
    {
        var user = AuthService.Instance.CurrentUser;
        if (user != null)
        {
            FirstNameEntry.Text = user.FirstName;
            LastNameEntry.Text = user.LastName;
            EmailEntry.Text = user.Email;
        }
    }

    private async void OnUpdateInfoClicked(object sender, EventArgs e)
    {
        var user = AuthService.Instance.CurrentUser;
        if (user == null) return;

        if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) ||
            string.IsNullOrWhiteSpace(LastNameEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Error", "Please fill in all fields.", "OK");
            return;
        }

        user.FirstName = FirstNameEntry.Text.Trim();
        user.LastName = LastNameEntry.Text.Trim();
        user.Email = EmailEntry.Text.Trim();

        var (success, message) = await AuthService.Instance.UpdateUserAsync(user);
        await DisplayAlert(success ? "Success" : "Error", message, "OK");
    }

    private async void OnChangePasswordClicked(object sender, EventArgs e)
    {
        var user = AuthService.Instance.CurrentUser;
        if (user == null) return;

        var newPass = NewPasswordEntry.Text;
        if (string.IsNullOrWhiteSpace(newPass))
        {
            await DisplayAlert("Error", "Enter new password.", "OK");
            return;
        }

        var (success, message) = await AuthService.Instance.ChangePasswordAsync(user.Id, newPass);
        if (success) NewPasswordEntry.Text = string.Empty;

        await DisplayAlert(success ? "Success" : "Error", message, "OK");
    }

    private async void OnDeleteAccountClicked(object sender, EventArgs e)
    {
        var user = AuthService.Instance.CurrentUser;
        if (user == null) return;

        bool confirm = await DisplayAlert("Delete Account",
            "Are you sure you want to delete your account? This action cannot be undone and all your data will be deleted.",
            "Yes, Delete", "Cancel");

        if (confirm)
        {
            await AuthService.Instance.DeleteUserAsync(user.Id);

            if (Application.Current?.Windows.Count > 0)
            {
                Application.Current.Windows[0].Page = new NavigationPage(new LoginPage());
            }
        }
    }


    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
