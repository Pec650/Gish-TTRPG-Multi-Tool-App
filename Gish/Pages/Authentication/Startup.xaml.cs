using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gish.Pages.Authentication;

public partial class Startup : ContentPage
{
    private List<Button> cachedButtons = new List<Button>();
    
    public Startup()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        App.setButtonState(cachedButtons, true);
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        cachedButtons = App.getAllButtons(this);
        
        App.setButtonState(cachedButtons, true);
    }

    private void RedirectToLogin(object? sender, EventArgs e)
    {
        App.setButtonState(cachedButtons, false);
        Navigation.PushAsync(new SignInPage());
    }

    private void RedirectToSignup(object? sender, EventArgs e)
    {
        App.setButtonState(cachedButtons, false);
        Navigation.PushAsync(new SignUpPage());
    }
}