using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace Gish.Pages.MainPages;

public partial class MainContainerPage : ContentPage
{
    private readonly View _homeView;
    private readonly View _creationsView;
    private readonly View _toolsView;
    private readonly View _rulesView;

    // This list will track history with "Home" permanently sitting at index 0
    private readonly List<string> _tabHistory = new();
    private bool _isInternalTabSwitch = false;

    public MainContainerPage()
    {
        InitializeComponent();

        _homeView = new HomeView(); 
        _creationsView = new CreationsView();
        _toolsView = new ToolsView();
        _rulesView = new RulesView();

        // Establish Home as the permanent absolute baseline anchor of your history stack
        _tabHistory.Add("Home");
        SwitchToTab("Home");
    }

 public void SwitchToTab(string tabName)
    {
        ContentArea.Children.Clear();

        if (!_isInternalTabSwitch)
        {
            if (tabName == "Home")
            {
                _tabHistory.Clear();
                _tabHistory.Add("Home");
            }
            else
            {
                _tabHistory.Remove(tabName);
                _tabHistory.Add(tabName);

                if (_tabHistory.Count > 4)
                {
                    _tabHistory.RemoveAt(1);
                }
            }
        }

        // Sync BOTH the bottom active states and top text title context
        AppTabBar.ActiveTab = tabName;
        AppHeader.Title = tabName; // <-- Syncs header text dynamically!

        switch (tabName)
        {
            case "Home":
                ContentArea.Children.Add(_homeView);
                break;
            case "Creations":
                ContentArea.Children.Add(_creationsView);
                break;
            case "Tools":
                ContentArea.Children.Add(_toolsView);
                break;
            case "Rules":
                ContentArea.Children.Add(_rulesView);
                break;
            case "Profile":
                // Support your ProfileView mapping when clicking the header icon
                AppHeader.Title = "My Profile";
                // ContentArea.Children.Add(_profileView); 
                break;
        }
    }

    public void SetTabBarVisibility(bool isVisible)
    {
        AppTabBar.IsVisible = isVisible;
        
        // Smoothly adjust content padding so scrolling fields take advantage of full-screen real estate when tabs are hidden
        ContentArea.Padding = isVisible ? new Thickness(0, 0, 0, 64) : new Thickness(0);
    }

    protected override bool OnBackButtonPressed()
    {
        // 1. Handle child leaf views inside specific tool sub-views first
        if (AppTabBar.ActiveTab == "Tools" && _toolsView is ToolsView tools)
        {
            if (tools.HasNestedViews) 
            {
                tools.PopLocalView();
                return true; // Handled locally!
            }
        }

        // 2. Handle Tab history backtracking
        if (_tabHistory.Count > 1)
        {
            // Remove the tab we are currently looking at from the end of the history list
            _tabHistory.RemoveAt(_tabHistory.Count - 1);

            // Peek at whatever tab is left at the end of the list (will be "Home" if it's the last one)
            string previousTab = _tabHistory[^1];

            _isInternalTabSwitch = true;
            SwitchToTab(previousTab);
            _isInternalTabSwitch = false;

            return true; // Intercepted the back button successfully!
        }

        // 3. If history is exactly 1 and Home is the active tab, explicitly quit the app
        if (_tabHistory.Count == 1 && AppTabBar.ActiveTab == "Home")
        {
            Application.Current?.Quit();
            return true; // You handled it — don't let MAUI also try to pop
        }

        // Final fallback: if things get weird, go home instead of crashing
        _isInternalTabSwitch = true;
        SwitchToTab("Home");
        _isInternalTabSwitch = false;
        return true;
    }
}