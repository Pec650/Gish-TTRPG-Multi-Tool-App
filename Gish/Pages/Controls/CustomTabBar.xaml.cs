using System;
using Microsoft.Maui.Controls;
using Gish.Pages.MainPages;

namespace Gish.Pages.Controls;

public partial class CustomTabBar : ContentView
{
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
        
        Dispatcher.Dispatch(() => {
            SetSelectedTab(ActiveTab ?? "Home");
        });
    }

    private static void OnActiveTabChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomTabBar tabBar && newValue is string tabName)
        {
            tabBar.SetSelectedTab(tabName);
        }
    }

    private void OnTabTapped(object sender, EventArgs e)
    {
        // Tap gesture target extraction checks
        if (sender is Element element)
        {
            // If the gesture target doesn't house the ID directly, crawl up to its element frame (VerticalStackLayout)
            string targetTab = element.AutomationId ?? element.Parent?.AutomationId;

            if (!string.IsNullOrEmpty(targetTab))
            {
                if (Application.Current?.MainPage is MainContainerPage mainContainer)
                {
                    // Ensure this calls your actual view frame swapping method name
                    mainContainer.SwitchToTab(targetTab);
                }
            }
        }
    }

    public void SetSelectedTab(string tabName)
    {
        var mutedColor = Color.FromArgb("#5D6660"); 
        HomeLabel.TextColor = mutedColor;
        CreationsLabel.TextColor = mutedColor;
        ToolsLabel.TextColor = mutedColor;
        RulesLabel.TextColor = mutedColor;
        
        HomeIcon.Opacity = 0.5;
        CreationsIcon.Opacity = 0.5;
        ToolsIcon.Opacity = 0.5;
        RulesIcon.Opacity = 0.5;

        HomeHighlight.Opacity = 0;
        HomeHighlight.Scale = 0.8;
        CreationsHighlight.Opacity = 0;
        CreationsHighlight.Scale = 0.8;
        ToolsHighlight.Opacity = 0;
        ToolsHighlight.Scale = 0.8;
        RulesHighlight.Opacity = 0;
        RulesHighlight.Scale = 0.8;

        var activeColor = Color.FromArgb("#1E2420"); 
        
        switch (tabName)
        {
            case "Home":
                HomeLabel.TextColor = activeColor;
                HomeIcon.Opacity = 1.0;
                HomeHighlight.FadeTo(1, 150);
                HomeHighlight.ScaleTo(1.0, 150, Easing.CubicOut);
                break;
            case "Creations":
                CreationsLabel.TextColor = activeColor;
                CreationsIcon.Opacity = 1.0;
                CreationsHighlight.FadeTo(1, 150);
                CreationsHighlight.ScaleTo(1.0, 150, Easing.CubicOut);
                break;
            case "Tools":
                ToolsLabel.TextColor = activeColor;
                ToolsIcon.Opacity = 1.0;
                ToolsHighlight.FadeTo(1, 150);
                ToolsHighlight.ScaleTo(1.0, 150, Easing.CubicOut);
                break;
            case "Rules":
                RulesLabel.TextColor = activeColor;
                RulesIcon.Opacity = 1.0;
                RulesHighlight.FadeTo(1, 150);
                RulesHighlight.ScaleTo(1.0, 150, Easing.CubicOut);
                break;
        }
    }
}