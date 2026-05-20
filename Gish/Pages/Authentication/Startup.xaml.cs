using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

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
        // Clean swap: No navigation wrappers involved
        App.SetMainPage(new SignInPage());
    }

    private void RedirectToSignup(object? sender, EventArgs e)
    {
        App.setButtonState(cachedButtons, false);
        // Clean swap
        App.SetMainPage(new SignUpPage());
    }
}