using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace Gish.Pages.Authentication;

public partial class StartupView : ContentView
{
    private List<Button> cachedButtons = new();
    
    public StartupView()
    {
        InitializeComponent();
        
        this.Loaded += (s, e) => App.setButtonState(cachedButtons, true);
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
        if (Parent is Grid parentGrid && parentGrid.Parent is AuthContainerPage container)
        {
            container.SwitchToAuthView("SignIn");
        }
    }

    private void RedirectToSignup(object? sender, EventArgs e)
    {
        App.setButtonState(cachedButtons, false);
        if (Parent is Grid parentGrid && parentGrid.Parent is AuthContainerPage container)
        {
            container.SwitchToAuthView("SignUp");
        }
    }
}