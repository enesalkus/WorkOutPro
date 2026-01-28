using WorkOutPro.Pages;
using WorkOutPro.Services;

namespace WorkOutPro;

public class MainTabbedPage : TabbedPage
{
    public MainTabbedPage()
    {
        Title = "WorkOutPro";

        // Set tab bar colors
        BarBackgroundColor = Color.FromArgb("#6C5CE7");
        BarTextColor = Colors.White;
        UnselectedTabColor = Color.FromArgb("#A29BFE");
        SelectedTabColor = Colors.White;

        // Helper to create styled NavigationPage
        NavigationPage CreateNavPage(Page page, string title, string icon)
        {
            // Set inner page Title to empty so it doesn't show on the NavBar (since Tab has title)
            // But wait, the NavBar title comes from the current page. If we want to hide it, we can set Title="" on page.
            // AND we set the NavigationPage title for the TAB.
            page.Title = "";

            // Add Global Toolbar Item based on Role
            if (AuthService.Instance.IsAdmin)
            {
                page.ToolbarItems.Add(new ToolbarItem("Admin", null, async () =>
                    await page.Navigation.PushModalAsync(new NavigationPage(new AdminUsersPage())))
                { Order = ToolbarItemOrder.Primary });
            }
            else
            {
                page.ToolbarItems.Add(new ToolbarItem("Account", null, async () =>
                    await page.Navigation.PushModalAsync(new NavigationPage(new AccountPage())))
                { Order = ToolbarItemOrder.Primary });
            }

            var nav = new NavigationPage(page)
            {
                Title = title,
                IconImageSource = icon,
                BarBackgroundColor = Color.FromArgb("#6C5CE7"),
                BarTextColor = Colors.White
            };
            return nav;
        }

        Children.Add(CreateNavPage(new MainPage(), "Home", "home.png"));
        Children.Add(CreateNavPage(new ExerciseTypesPage(), "Exercises", "dumbbell.png"));
        Children.Add(CreateNavPage(new TrainingSessionsPage(), "Sessions", "calendar.png"));
        Children.Add(CreateNavPage(new TrainingExercisesPage(), "Workouts", "run.png"));
        Children.Add(CreateNavPage(new StatsPage(), "Stats", "chart.png"));
    }
}
