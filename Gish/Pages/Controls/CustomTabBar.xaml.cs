using System;
using Microsoft.Maui.Controls;
using Gish.Pages.MainPages;

namespace Gish.Pages.Controls;

public partial class CustomTabBar : ContentView
{
    // Registering the property so it can be set in XAML attributes
    public static readonly BindableProperty ActiveTabProperty =
        BindableProperty.Create(
            nameof(ActiveTab),
            typeof(string),
            typeof(CustomTabBar),
            default(string),
            propertyChanged: OnActiveTabChanged);

    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    public CustomTabBar()
    {
        InitializeComponent();
    }

    private static void OnActiveTabChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomTabBar tabBar && newValue is string tabName)
        {
            tabBar.SetSelectedTab(tabName);
        }
    }

    private void OnHomeTapped(object sender, TappedEventArgs e)
    {
        App.SetMainPage(new HomePage());
    }

    private void OnCreationsTapped(object sender, TappedEventArgs e)
    {
        App.SetMainPage(new CreationsPage());
    }

    private void OnToolsTapped(object sender, TappedEventArgs e)
    {
        App.SetMainPage(new ToolsPage());
    }

    private void OnRulesTapped(object sender, TappedEventArgs e)
    {
        App.SetMainPage(new RulesPage());
    }

    public void SetSelectedTab(string tabName)
    {
        // Reset all text colors to your default dark tone
        HomeLabel.TextColor = Color.FromArgb("#424C44");
        CreationsLabel.TextColor = Color.FromArgb("#424C44");
        ToolsLabel.TextColor = Color.FromArgb("#424C44");
        RulesLabel.TextColor = Color.FromArgb("#424C44");
        
        // Reset all icon visibilities to dim state
        HomeIcon.Opacity = 0.6;
        CreationsIcon.Opacity = 0.6;
        ToolsIcon.Opacity = 0.6;
        RulesIcon.Opacity = 0.6;

        var activeColor = Color.FromArgb("#1E2420"); 
        switch (tabName)
        {
            case "Home":
                HomeLabel.TextColor = activeColor;
                HomeIcon.Opacity = 1.0;
                break;
            case "Creations":
                CreationsLabel.TextColor = activeColor;
                CreationsIcon.Opacity = 1.0;
                break;
            case "Tools":
                ToolsLabel.TextColor = activeColor;
                ToolsIcon.Opacity = 1.0;
                break;
            case "Rules":
                RulesLabel.TextColor = activeColor;
                RulesIcon.Opacity = 1.0;
                break;
        }
    }
}