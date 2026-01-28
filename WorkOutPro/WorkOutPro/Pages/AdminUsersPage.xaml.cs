using WorkOutPro.Services;
using WorkOutPro.Models;

namespace WorkOutPro.Pages;

public partial class AdminUsersPage : ContentPage
{
    public AdminUsersPage()
    {
        InitializeComponent();
        LoadUsers();
    }

    private void LoadUsers()
    {
        // Avoid listing self if you want, but standard is list all
        var users = AuthService.Instance.GetAllUsers();
        UsersList.ItemsSource = users;
    }

    private async void OnDeleteUserClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is User user)
        {
            // Prevent deleting self? Or warn.
            if (user.Id == AuthService.Instance.CurrentUser?.Id)
            {
                await DisplayAlert("Error", "Admin account cannot be deleted.", "OK");
                return;
            }

            bool confirm = await DisplayAlert("Delete User",
                $"Are you sure you want to delete {user.FullName} ({user.Email})?",
                "Yes", "No");

            if (confirm)
            {
                await AuthService.Instance.DeleteUserAsync(user.Id);
                LoadUsers();
            }
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
