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

    public void PushLocalView(ContentView childView)
    {
        _localNavigationStack.Push(childView);
        DefaultToolsMenu.IsVisible = false;
        NestedStackPresenter.Content = childView;
        NestedStackPresenter.IsVisible = true;

        if (Application.Current?.Windows?[0]?.Page is MainContainerPage mainContainer)
        {
            // 1. Let the data binding update the text safely through the property channel
            mainContainer.AppHeader.Title = "Session Reminder"; 
            
            // 2. Toggle only the structural arrow layout states
            mainContainer.AppHeader.SetNavigationMode(isNavMode: true, backAction: () => PopLocalView());
            
            mainContainer.SetTabBarVisibility(false); 
        }
    }

    public void PopLocalView()
    {
        if (_localNavigationStack.Count > 0)
        {
            _localNavigationStack.Pop();
        }

        if (_localNavigationStack.Count > 0)
        {
            NestedStackPresenter.Content = _localNavigationStack.Peek();
        }
        else
        {
            NestedStackPresenter.Content = null;
            NestedStackPresenter.IsVisible = false;
            DefaultToolsMenu.IsVisible = true;

            if (Application.Current?.Windows?[0]?.Page is MainContainerPage mainContainer)
            {
                mainContainer.AppHeader.Title = "Tools";
                mainContainer.AppHeader.SetNavigationMode(isNavMode: false);
                
                // Cleanly restores visibility without causing layout calculation stuttering
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
        PushLocalView(schedulerView);
    }
}