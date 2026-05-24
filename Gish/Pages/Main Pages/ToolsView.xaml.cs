using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Gish.Pages.ToolPages;

namespace Gish.Pages.MainPages;

public partial class ToolsView : ContentView
{
    private readonly Stack<ContentView> _localNavigationStack = new();

    public bool HasNestedViews => _localNavigationStack != null && _localNavigationStack.Count > 0;

    public ToolsView()
    {
        InitializeComponent();
    }

    public void PushLocalView(ContentView childView, string viewTitle)
    {
        _localNavigationStack.Push(childView);
        DefaultToolsMenu.IsVisible = false;
        NestedStackPresenter.Content = childView;
        NestedStackPresenter.IsVisible = true;

        if (Application.Current?.Windows?[0]?.Page is MainContainerPage mainContainer)
        {
            // Update the text safely using the passed parameter
            mainContainer.AppHeader.Title = viewTitle; 
            
            // Toggle only the structural arrow layout states
            mainContainer.AppHeader.SetNavigationMode(isNavMode: true, backAction: () => PopLocalView());
            
            mainContainer.SetTabBarVisibility(false); 
        }
    }

    // Example if tracking titles via a tuple stack or simple type checking:
    public void PopLocalView()
    {
        if (_localNavigationStack.Count > 0)
        {
            _localNavigationStack.Pop();
        }

        if (Application.Current?.Windows?[0]?.Page is MainContainerPage mainContainer)
        {
            if (_localNavigationStack.Count > 0)
            {
                var previousView = _localNavigationStack.Peek();
                NestedStackPresenter.Content = previousView;
                
                // Set title based on what view we returned to
                mainContainer.AppHeader.Title = previousView is HomeView ? "Session Reminder" : "Tools";
            }
            else
            {
                // Reached the base layout level
                NestedStackPresenter.IsVisible = false;
                DefaultToolsMenu.IsVisible = true;
                
                mainContainer.AppHeader.Title = "Tools"; 
                mainContainer.AppHeader.SetNavigationMode(isNavMode: false, backAction: null);
                mainContainer.SetTabBarVisibility(true);
            }
        }
    }

    private void UpdateParentTabBarVisibility()
    {
        if (Application.Current?.MainPage is MainContainerPage mainContainer)
        {
            // If we have nested items on screen (SchedulerView, FormView, etc), hide the bar.
            // Otherwise, make it visible.
            mainContainer.SetTabBarVisibility(!HasNestedViews);
        }
    }

    private void OnSessionScheduleClicked(object sender, EventArgs e)
    {
        var schedulerView = new SchedulerView();
        PushLocalView(schedulerView, "Session Reminder");
    }

    private void OnCRCalculatorClicked(object sender, EventArgs e)
    {
        var crCalculatorView = new CRCalculatorView();
        PushLocalView(crCalculatorView, "CR Calculator");
    }
}