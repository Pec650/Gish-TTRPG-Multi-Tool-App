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
        
        // Hide only the inner tools selector menu grid
        DefaultToolsMenu.IsVisible = false;
        
        // Mount and show the custom tool scheduler component safely
        NestedStackPresenter.Content = childView;
        NestedStackPresenter.IsVisible = true;
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
            // Fully return and clear back states safely
            NestedStackPresenter.Content = null;
            NestedStackPresenter.IsVisible = false;
            DefaultToolsMenu.IsVisible = true;
        }
    }

    private void OnSessionScheduleClicked(object sender, EventArgs e)
    {
        var schedulerView = new SchedulerView();
        PushLocalView(schedulerView);
    }
}