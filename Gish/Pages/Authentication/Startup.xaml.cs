namespace Gish.Pages.Authentication;

public partial class Startup
{
    public Startup()
    {
        InitializeComponent();
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