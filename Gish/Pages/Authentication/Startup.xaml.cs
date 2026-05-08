using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gish.Pages.Authentication;

public partial class Startup : ContentPage
{
    public Startup()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        GoToLoginBtn.IsEnabled = true;
        GoToSignUpBtn.IsEnabled = true;
    }

    private void RedirectToLogin(object? sender, EventArgs e)
    {
        GoToLoginBtn.IsEnabled = false;
        Navigation.PushAsync(new SignInPage());
    }

    private void RedirectToSignup(object? sender, EventArgs e)
    {
        GoToSignUpBtn.IsEnabled = false;
        Navigation.PushAsync(new SignUpPage());
    }
}