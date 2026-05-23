using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Gish.Pages.Classes;
using Gish.Pages.MainPages;

namespace Gish.Pages.ToolPages;

public partial class SchedulerView : ContentView
{
    private readonly LocalDatabase _database = new();
    private DateTime _selectedDate;
    private List<GameSession> _allSessions = new();
    private Border? _currentlySelectedDayView;
    private GameSession? _activeSelectedSession;

    public SchedulerView()
    {
        InitializeComponent();
        _selectedDate = new DateTime(2026, 5, 1);
    }

    protected override async void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler is not null)
        {
            _allSessions = await _database.GetAllSessionsAsync();
            BuildCalendar();
        }
    }

    private void BuildCalendar()
    {
        CalendarGrid.Children.Clear();
        MonthYearLabel.Text = _selectedDate.ToString("MMMM yyyy");

        int daysInMonth = DateTime.DaysInMonth(_selectedDate.Year, _selectedDate.Month);
        int dayOfWeekOffset = (int)new DateTime(_selectedDate.Year, _selectedDate.Month, 1).DayOfWeek;

        int currentColumn = dayOfWeekOffset;
        int currentRow = 0;

        for (int i = 0; i < dayOfWeekOffset; i++)
        {
            var placeholder = new Border { BackgroundColor = Color.FromArgb("#EAECEB"), StrokeShape = new RoundRectangle { CornerRadius = 4 }, Opacity = 0.5 };
            Grid.SetColumn(placeholder, i);
            Grid.SetRow(placeholder, 0);
            CalendarGrid.Children.Add(placeholder);
        }

        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime targetDate = new DateTime(_selectedDate.Year, _selectedDate.Month, day);
            bool hasSession = _allSessions.Any(s => s.Date.Date == targetDate.Date);

            var dayBorder = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 4 },
                Padding = 0,
                HeightRequest = 44,
                BackgroundColor = hasSession ? Color.FromArgb("#A2C9B4") : Color.FromArgb("#EAECEB"), 
                Stroke = Color.FromRgba(0,0,0,0),
                StrokeThickness = 2
            };

            var dayLabel = new Label
            {
                Text = day.ToString(),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontFamily = "Faustina-Bold.ttf",
                FontSize = 14,
                TextColor = Color.FromArgb("#1E2420")
            };

            dayBorder.Content = dayLabel;

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnDayTapped(dayBorder, targetDate);
            dayBorder.GestureRecognizers.Add(tapGesture);

            Grid.SetColumn(dayBorder, currentColumn);
            Grid.SetRow(dayBorder, currentRow);
            CalendarGrid.Children.Add(dayBorder);

            currentColumn++;
            if (currentColumn > 6)
            {
                currentColumn = 0;
                currentRow++;
            }
        }
    }

    private async void OnDayTapped(Border selectedBorder, DateTime date)
    {
        _selectedDate = date; // Lock in the active calendar day target tracking state

        // 1. Shift the calendar cell focus highlight border stroke accent
        if (_currentlySelectedDayView is not null)
        {
            _currentlySelectedDayView.Stroke = Color.FromRgba(0, 0, 0, 0);
        }
        _currentlySelectedDayView = selectedBorder;
        _currentlySelectedDayView.Stroke = Color.FromArgb("#E8A984"); 

        // 2. Fetch all sessions occurring on this chosen day from the database
        var daySessions = await _database.GetSessionsForDayAsync(date);

        // 3. Reset and clear the dynamic container list view layout
        DrawerSessionsContainer.Children.Clear();
        DrawerHeaderLabel.Text = date.ToString("MMMM d, yyyy");
        DetailsDrawer.IsVisible = true; // Always display the selection drawer utility now!

        if (daySessions.Count == 0)
        {
            // Display a clean inline helper prompt showing there are no bookings yet
            DrawerSessionsContainer.Children.Add(new Label 
            { 
                Text = "No game sessions scheduled for this day.", 
                TextColor = Color.FromArgb("#5D6660"),
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 15)
            });
            return;
        }

        // 4. Populate card elements for each session found on this calendar day
        foreach (var session in daySessions)
        {
            var card = new Border
            {
                BackgroundColor = Color.FromArgb("#D4C5B3"),
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Padding = 15,
                Stroke = Color.FromRgba(0,0,0,0),
                Margin = new Thickness(0, 2)
            };

            var grid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition { Width = GridLength.Auto }, new ColumnDefinition { Width = GridLength.Star } },
                RowDefinitions = { new RowDefinition { Height = GridLength.Auto }, new RowDefinition { Height = GridLength.Auto } },
                ColumnSpacing = 15
            };

            var icon = new Image { Source = "discord_icon.png", WidthRequest = 32, HeightRequest = 32, VerticalOptions = LayoutOptions.Center };
            Grid.SetRowSpan(icon, 2);
            grid.Children.Add(icon);

            var titleLabel = new Label { Text = session.Title, FontFamily = "Faustina-Bold.ttf", FontSize = 18, TextColor = Color.FromArgb("#1E2420") };
            Grid.SetColumn(titleLabel, 1);
            grid.Children.Add(titleLabel);

            var timeLabel = new Label { Text = $"{DateTime.Today.Add(session.StartTime):h:mm tt}", FontSize = 12, TextColor = Color.FromArgb("#5D6660"), HorizontalOptions = LayoutOptions.End };
            Grid.SetColumn(timeLabel, 1);
            grid.Children.Add(timeLabel);

            card.Content = grid;

            // Wire tap input event gestures to allow instant editing inside individual session list cards
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => OpenFormForEdit(session);
            card.GestureRecognizers.Add(tap);

            DrawerSessionsContainer.Children.Add(card);
        }
    }

    private void OnDrawerAddSessionClicked(object sender, EventArgs e)
    {
        // Route user directly to the session creation form view, prefilled with the locked date selection context
        var formView = new SchedulerFormView(_selectedDate);
        
        formView.OnDatabaseChanged = async () => {
            _allSessions = await _database.GetAllSessionsAsync();
            BuildCalendar();
            DetailsDrawer.IsVisible = false;
        };

        NavigateToForm(formView);
    }

    private void OnPreviousMonthClicked(object sender, EventArgs e)
    {
        _selectedDate = _selectedDate.AddMonths(-1);
        DetailsDrawer.IsVisible = false;
        BuildCalendar();
    }

    private void OnNextMonthClicked(object sender, EventArgs e)
    {
        _selectedDate = _selectedDate.AddMonths(1);
        DetailsDrawer.IsVisible = false;
        BuildCalendar();
    }

    private void OnBackClicked(object sender, EventArgs e)
    {
    // Find the structural parent wrapper host view
        Element currentParent = this.Parent;
        while (currentParent is not null && currentParent is not ToolsView)
        {
            currentParent = currentParent.Parent;
        }
        
        // Check if we found the true local stack owner
        if (currentParent is ToolsView toolsMenuHost)
        {
            // Safely pull this page off the custom internal local view stack
            toolsMenuHost.PopLocalView();
        }
    }

    private void OnCreateSessionClicked(object sender, EventArgs e)
    {
        DateTime fallbackDate = _currentlySelectedDayView is not null ? _selectedDate : DateTime.Today;
        var formView = new SchedulerFormView(fallbackDate);
        
        formView.OnDatabaseChanged = async () => {
            _allSessions = await _database.GetAllSessionsAsync();
            BuildCalendar();
        };

        NavigateToForm(formView);
    }

    private void OpenFormForEdit(GameSession session)
    {
        var formView = new SchedulerFormView(session);
        formView.OnDatabaseChanged = async () => {
            _allSessions = await _database.GetAllSessionsAsync();
            BuildCalendar();
            DetailsDrawer.IsVisible = false;
        };

        NavigateToForm(formView);
    }

    private void NavigateToForm(ContentView form)
    {
        Element currentParent = this.Parent;
        while (currentParent is not null && currentParent is not ToolsView)
        {
            currentParent = currentParent.Parent;
        }

        if (currentParent is ToolsView toolsMenuHost)
        {
            toolsMenuHost.PushLocalView(form);
        }
    }
}