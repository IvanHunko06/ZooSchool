namespace Client;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(LessonPage), typeof(LessonPage));
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(TestPage), typeof(TestPage));

        Navigated += AppShell_Navigated;
    }

    private void AppShell_Navigated(object? sender, ShellNavigatedEventArgs e)
    {
        Shell.SetNavBarIsVisible(this, false);
    }
}
