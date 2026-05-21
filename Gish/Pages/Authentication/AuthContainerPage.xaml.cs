namespace Gish.Pages.Authentication;

public partial class AuthContainerPage : ContentPage
{
    public AuthContainerPage()
    {
        InitializeComponent();
        // Load the initial view
        SwitchToAuthView("Startup");
    }

    public void SwitchToAuthView(string viewName)
    {
        // Clear whatever sub-view is currently sitting inside your grid
        AuthContentArea.Children.Clear();

        // Inject the new view into your existing layout area
        switch (viewName)
        {
            case "Startup":
                AuthContentArea.Children.Add(new StartupView());
                break;
            case "SignIn":
                AuthContentArea.Children.Add(new SignInView());
                break;
            case "SignUp":
                AuthContentArea.Children.Add(new SignUpView());
                break;
        }
    }
}