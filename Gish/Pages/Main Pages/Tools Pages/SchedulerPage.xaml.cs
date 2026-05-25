using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Gish.Pages.Classes;
using Gish.Pages.Main_Pages;

namespace Gish.Pages.Main_Pages.Tools_Pages;

public partial class SchedulerPage : ContentPage
{
    private readonly LocalDatabase _database = new();
    private DateTime _selectedDate;
    private List<GameSession> _allSessions = new();
    private Border? _currentlySelectedDayView;
    private GameSession? _activeSelectedSession;

    public SchedulerPage()
    {
        InitializeComponent();
        _selectedDate = new DateTime(2026, 5, 1);
        BuildCalendar();
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
        
        DateTime today = DateTime.Today;

        for (int i = 0; i < dayOfWeekOffset; i++)
        {
            var placeholder = new Border { BackgroundColor = Color.FromArgb("#D9D9D9"), StrokeShape = new RoundRectangle { CornerRadius = 4 }, Opacity = 0.5 };
            Grid.SetColumn(placeholder, i);
            Grid.SetRow(placeholder, 0);
            CalendarGrid.Children.Add(placeholder);
        }

        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime targetDate = new DateTime(_selectedDate.Year, _selectedDate.Month, day);
            bool hasSession = _allSessions.Any(s => s.Date.Date == targetDate.Date);
            bool isToday = targetDate.Date == today;

            var dayBorder = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 4 },
                Padding = 0,
                HeightRequest = 44,
                BackgroundColor = hasSession ? Color.FromArgb("#738ADB") : Colors.White, 
                Stroke = Color.FromRgba(0,0,0,0),
                StrokeThickness = 3
            };

            var dayLabel = new Label
            {
                Text = day.ToString(),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontFamily = "Faustina-Bold.ttf",
                FontSize = 17,
                TextColor = Color.FromArgb("#1E2420"),
                FontAttributes = FontAttributes.Bold
            };
            
            if (currentColumn % 7 == 0 && !hasSession)
            {
                dayBorder.BackgroundColor = Color.FromArgb("#D4956A");
                dayLabel.TextColor = Colors.White;
            }
            
            if (isToday)
            {
                dayBorder.BackgroundColor = Color.FromArgb("A2C9B4");
                dayLabel.TextColor = Color.FromArgb("#1E2420");
            }

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

        if (currentColumn > 0)
        {
            for (int i = currentColumn; i <= 6; i++)
            {
                var placeholder = new Border { BackgroundColor = Color.FromArgb("#D9D9D9"), StrokeShape = new RoundRectangle { CornerRadius = 4 }, Opacity = 0.5 };
                Grid.SetColumn(placeholder, i);
                Grid.SetRow(placeholder, currentRow);
                CalendarGrid.Children.Add(placeholder);
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
        _currentlySelectedDayView.Stroke = Color.FromArgb("#1E2420"); 

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
                TextColor = Colors.White,
                FontSize = 15,
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
                BackgroundColor = Color.FromArgb("#E8F0D8"),
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Padding = 15,
                Stroke = Color.FromRgba(0,0,0,0),
                Margin = new Thickness(0, 2)
            };

            var grid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition { Width = GridLength.Auto }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Auto } },
                ColumnSpacing = 15
            };

            var icon = new Image { Source = "discord_icon.png", WidthRequest = 32, HeightRequest = 32, VerticalOptions = LayoutOptions.Center };
            Grid.SetColumn(icon, 0);
            grid.Children.Add(icon);

            var titleLabel = new Label { Text = session.Title, FontFamily = "Faustina-Bold.ttf", FontSize = 20, TextColor = Color.FromArgb("#1E2420"), LineBreakMode=LineBreakMode.TailTruncation, MaxLines=1, WidthRequest=250, HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.Fill};
            Grid.SetColumn(titleLabel, 1);
            grid.Children.Add(titleLabel);

            var timeLabel = new Label { Text = $"{DateTime.Today.Add(session.StartTime):h:mm tt}", FontSize = 15, TextColor = Color.FromArgb("#1E2420"), FontAttributes = FontAttributes.Bold,HorizontalOptions = LayoutOptions.End };
            Grid.SetColumn(timeLabel, 2);
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
        var formPage = new SchedulerFormPage(_selectedDate);
        
        formPage.OnDatabaseChanged = async () => {
            _allSessions = await _database.GetAllSessionsAsync();
            BuildCalendar();
            DetailsDrawer.IsVisible = false;
        };
        
        NavigateToForm(formPage);
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

    private void OnCreateSessionClicked(object sender, EventArgs e)
    {
        DateTime fallbackDate = _currentlySelectedDayView is not null ? _selectedDate : DateTime.Today;
        var formPage = new SchedulerFormPage(fallbackDate);
        
        formPage.OnDatabaseChanged = async () => {
            _allSessions = await _database.GetAllSessionsAsync();
            BuildCalendar();
        };
        
        NavigateToForm(formPage);
    }

    private void OpenFormForEdit(GameSession session)
    {
        var formPage = new SchedulerFormPage(session);
        formPage.OnDatabaseChanged = async () => {
            _allSessions = await _database.GetAllSessionsAsync();
            BuildCalendar();
            DetailsDrawer.IsVisible = false;
        };
        
        NavigateToForm(formPage);
    }

    private async void NavigateToForm(ContentPage formPage)
    {
        try
        {
            // Pushes the form page cleanly as a modal view sheet
            await Navigation.PushModalAsync(formPage);
        } 
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation failed: {ex.Message}");
        }
    }

    private void ReturnPage(object? sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}