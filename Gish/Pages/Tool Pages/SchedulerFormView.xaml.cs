using System;
using Microsoft.Maui.Controls;
using Gish.Pages.Classes;
using Gish.Pages.MainPages;

namespace Gish.Pages.ToolPages;

public partial class SessionFormView : ContentView
{
    private readonly LocalDatabase _database = new();
    private readonly GameSession? _existingSession;
    private readonly DateTime? _preSelectedDate;

    // Delegate callback to instruct SchedulerView to rebuild its grid after database row modifications
    public Action? OnDatabaseChanged { get; set; }

    // Constructor Variant A: Creating a blank entry row
    public SessionFormView(DateTime? preSelectedDate = null)
    {
        InitializeComponent();
        _preSelectedDate = preSelectedDate;
        
        if (_preSelectedDate.HasValue)
        {
            SessionDatePicker.Date = _preSelectedDate.Value;
        }
    }

    // Constructor Variant B: Modifying / Deleting an existing session layout
    public SessionFormView(GameSession session)
    {
        InitializeComponent();
        _existingSession = session;

        // Populate fields with current database model data
        FormHeaderTitle.Text = "Edit Session Data";
        DeleteButton.IsVisible = true;

        TitleEntry.Text = session.Title;
        CampaignEntry.Text = session.CampaignTitle;
        SystemEntry.Text = session.RPGSystem;
        SessionDatePicker.Date = session.Date;
        SessionTimePicker.Time = session.StartTime;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            await Application.Current!.MainPage!.DisplayAlertAsync("Missing Field", "Please enter a valid session title name.", "OK");
            return;
        }

        var sessionToSave = _existingSession ?? new GameSession();
        sessionToSave.Title = TitleEntry.Text;
        sessionToSave.CampaignTitle = CampaignEntry.Text;
        sessionToSave.RPGSystem = SystemEntry.Text;

        // FIX CS0266 HERE: Force explicit non-nullable extraction
        
        sessionToSave.Date = SessionDatePicker.Date ?? DateTime.Today;
        sessionToSave.StartTime = SessionTimePicker.Time ?? TimeSpan.Zero;

        await _database.SaveSessionAsync(sessionToSave);

        OnDatabaseChanged?.Invoke();
        CloseForm();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_existingSession is null) return;

        bool confirm = await Application.Current!.MainPage!.DisplayAlertAsync("Confirm Delete", "Are you sure you want to delete this session?", "Yes", "No");
        if (confirm)
        {
            await _database.DeleteSessionAsync(_existingSession);
            OnDatabaseChanged?.Invoke();
            CloseForm();
        }
    }

    private void OnCancelClicked(object sender, EventArgs e) => CloseForm();

    private void CloseForm()
    {
        // Crawl up the stack view frames to locate the root local stack operator host
        Element currentParent = this.Parent;
        while (currentParent is not null && currentParent is not ToolsView)
        {
            currentParent = currentParent.Parent;
        }

        if (currentParent is ToolsView toolsMenuHost)
        {
            toolsMenuHost.PopLocalView();
        }
    }
}