namespace Gish;

using Gish.Pages.Classes;

public partial class App : Application
{
    public static User CurrentUser = new User();
    
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
    
    public static void resetUser()
    {
        CurrentUser.emptyUser();
    }

    public static bool isLoggedIn()
    {
        return CurrentUser.userExists();
    }
}