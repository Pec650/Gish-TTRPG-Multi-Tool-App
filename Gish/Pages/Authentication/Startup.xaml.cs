using Gish.Pages.Main_Pages;

namespace Gish.Pages.Authentication;

public partial class Startup
{
    public Startup()
    {
        InitializeComponent();

        CheckIfLoggedIn();
    }

    private async void CheckIfLoggedIn()
    {
        try
        {
            bool loggedIn = await App.IsLoggedIn();

            if (loggedIn)
            {
                App.SetMainPage(new NavigationPage(new MainContainerPage()));
            }
        }
        catch
        {
            // ignored
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SetAllButtonState(true);
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        SetAllButtonState(true);
    }

    private void RedirectToLogin(object? sender, EventArgs e)
    {
        SetAllButtonState(false);
        Navigation.PushAsync(new SignInPage());
    }

    private void RedirectToSignup(object? sender, EventArgs e)
    {
        SetAllButtonState(false);
        Navigation.PushAsync(new SignUpPage());
    }

    private void SetAllButtonState(bool isEnable)
    {
        GoToLoginBtn.IsEnabled = isEnable;
        GoToSignUpBtn.IsEnabled = isEnable;
    }
}