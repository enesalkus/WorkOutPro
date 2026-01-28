using WorkOutPro.Pages;
using WorkOutPro.Services;

namespace WorkOutPro;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Seed default users
        Task.Run(async () =>
        {
            try
            {
                await AuthService.Instance.SeedDefaultUsersAsync();
            }
            catch { }
        });
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // TEST: Start directly with AppShell to see if it crashes
        // return new Window(new AppShell());

        // Normal: Start with login page
        return new Window(new NavigationPage(new LoginPage()));
    }
}