using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Gish.Pages.Classes;
using Gish.Pages.MainPages;

namespace Gish.Pages.Main_Pages.Tools_Pages;

public partial class SchedulerFormPage : ContentPage
{
    private readonly LocalDatabase _database = new();
    private GameSession _currentSession;
    private bool _isEditMode;

    // Direct object references to manage ID tracking behind the scenes
    private List<RPGSystem> _availableSystems = new();
    private List<Campaign> _availableCampaigns = new();

    private const string CustomOptionText = "+ Add Custom...";

    public Action? OnDatabaseChanged { get; set; }

    public SchedulerFormPage(DateTime defaultDate)
    {
        InitializeComponent();
        _currentSession = new GameSession
        {
            Date = defaultDate.Date,
            StartTime = new TimeSpan(13, 30, 0),
            EndTime = new TimeSpan(16, 30, 0)
        };
        _isEditMode = false;
        LoadFormFields();
    }

    public SchedulerFormPage(GameSession existingSession)
    {
        InitializeComponent();
        _currentSession = existingSession;
        _isEditMode = true;
        LoadFormFields();
    }

    private async void LoadFormFields()
    {
        // Double check that XAML elements exist
        if (SystemPicker == null || TitleEntry == null || CampaignPicker == null) return;

        try
        {
            // Temporarily unsubscribe from events to prevent infinite loops / layout updates
            SystemPicker.SelectedIndexChanged -= OnSystemPickerChanged;
            CampaignPicker.SelectedIndexChanged -= OnCampaignPickerChanged;

            // Populate base fields
            TitleEntry.Text = _currentSession.Title;
            SessionDatePicker.Date = _currentSession.Date;
            SessionTimePicker.Time = _currentSession.StartTime;

            // Fetch lookup items
            _availableSystems = await _database.GetAllSystemsAsync() ?? new List<RPGSystem>();

            SystemPicker.Items.Clear();
            foreach (var sys in _availableSystems)
            {
                SystemPicker.Items.Add(sys.Name);
            }
            SystemPicker.Items.Add(CustomOptionText);

            // Populate Edit Mode data safely
            if (_isEditMode && _currentSession.CampaignID != 0)
            {
                var campaigns = await _database._connection.Table<Campaign>().ToListAsync();
                var activeCampaign = campaigns.FirstOrDefault(c => c.ID == _currentSession.CampaignID);

                if (activeCampaign != null)
                {
                    var activeSystem = _availableSystems.FirstOrDefault(s => s.ID == activeCampaign.RPGSystemID);
                    if (activeSystem != null)
                    {
                        SystemPicker.SelectedItem = activeSystem.Name;
                        
                        // Await the data retrieval completely BEFORE updating picker items
                        await RefreshCampaignListAsync(activeSystem.ID);
                        
                        CampaignPicker.SelectedItem = activeCampaign.Title;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading form fields: {ex.Message}");
        }
        finally
        {
            // Re-subscribe to events so user interaction works normally again
            SystemPicker.SelectedIndexChanged += OnSystemPickerChanged;
            CampaignPicker.SelectedIndexChanged += OnCampaignPickerChanged;
        }
    }

    private async Task RefreshCampaignListAsync(int systemId)
    {
        // _availableCampaigns = await _database.GetCampaignsBySystemAsync(systemId);
        //
        // CampaignPicker.Items.Clear();
        // foreach (var camp in _availableCampaigns)
        // {
        //     CampaignPicker.Items.Add(camp.Title);
        // }
        // CampaignPicker.Items.Add(CustomOptionText);
    }

    private async void OnSystemPickerChanged(object sender, EventArgs e)
    {
        // if (SystemPicker.SelectedIndex == -1) return;
        //
        // string selectedText = SystemPicker.Items[SystemPicker.SelectedIndex];
        //
        // if (selectedText == CustomOptionText)
        // {
        //     CustomSystemEntry.IsVisible = true;
        //     CampaignPicker.Items.Clear();
        //     CampaignPicker.Items.Add(CustomOptionText);
        //     CampaignPicker.SelectedItem = CustomOptionText;
        // }
        // else
        // {
        //     CustomSystemEntry.IsVisible = false;
        //     var targetSys = _availableSystems.FirstOrDefault(s => s.Name == selectedText);
        //     if (targetSys != null)
        //     {
        //         await RefreshCampaignListAsync(targetSys.ID);
        //     }
        // }
    }

    private void OnCampaignPickerChanged(object sender, EventArgs e)
    {
        // if (CampaignPicker.SelectedIndex == -1) return;
        // string selectedText = CampaignPicker.Items[CampaignPicker.SelectedIndex];
        // CustomCampaignEntry.IsVisible = (selectedText == CustomOptionText);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string sessionTitle = string.IsNullOrWhiteSpace(TitleEntry.Text) ? "Untitled Session" : TitleEntry.Text.Trim();
        _currentSession.Title = sessionTitle;
        _currentSession.Date = SessionDatePicker.Date ?? DateTime.Today;
        _currentSession.StartTime = SessionTimePicker.Time ?? TimeSpan.Zero;

        try
        {
            int resolvedSystemId = 0;
            string sysName = "";

            if (SystemPicker.SelectedItem?.ToString() == CustomOptionText)
            {
                sysName = CustomSystemEntry.Text?.Trim() ?? "Custom System";
                var newSys = new RPGSystem { Name = sysName };
                await _database._connection.InsertAsync(newSys);
                resolvedSystemId = newSys.ID;
            }
            else if (SystemPicker.SelectedIndex != -1)
            {
                sysName = SystemPicker.Items[SystemPicker.SelectedIndex];
                var sysObj = _availableSystems.FirstOrDefault(s => s.Name == sysName);
                resolvedSystemId = sysObj?.ID ?? 0;
            }

            int resolvedCampaignId = 0;
            string campaignTitle = "Custom Campaign";

            if (CampaignPicker.SelectedItem?.ToString() == CustomOptionText)
            {
                campaignTitle = CustomCampaignEntry.Text?.Trim() ?? "Custom Campaign";
                var newCamp = new Campaign { Title = campaignTitle, RPGSystemID = resolvedSystemId };
                await _database._connection.InsertAsync(newCamp);
                resolvedCampaignId = newCamp.ID;
            }
            else if (CampaignPicker.SelectedIndex != -1)
            {
                campaignTitle = CampaignPicker.Items[CampaignPicker.SelectedIndex];
                var campObj = _availableCampaigns.FirstOrDefault(c => c.Title == campaignTitle && c.RPGSystemID == resolvedSystemId);
                resolvedCampaignId = campObj?.ID ?? 0;
            }

            _currentSession.CampaignID = resolvedCampaignId;

            // Save FIRST so SessionID is assigned before we use it as the tracking key
            if (!_isEditMode)
                await _database.SaveSessionWithValidationAsync(_currentSession);

            // Now SessionID is guaranteed to be populated for both new and edited sessions
            string trackingId = _currentSession.SessionID.ToString();

            bool isReminderEnabled = ReminderSwitch.IsToggled;

            if (isReminderEnabled)
            {
                DateTime sessionStart = _currentSession.Date.Date + _currentSession.StartTime;
                DateTime notifyTime = sessionStart.AddMinutes(-30);
            }

            OnDatabaseChanged?.Invoke();
            GoBack();
        }
        catch (InvalidOperationException ex)
        {
            var mainPage = Application.Current?.Windows?[0]?.Page;
            if (mainPage is not null)
            {
                await mainPage.DisplayAlertAsync("Scheduling Conflict", ex.Message, "OK");
            }
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
         if (_isEditMode && _currentSession is not null)
        {
            var mainPage = Application.Current?.Windows?[0]?.Page;
            if (mainPage is not null && await mainPage.DisplayAlertAsync("Delete Session", "Remove this session?", "Yes", "No"))
            {
                string trackingId = _currentSession.SessionID.ToString();
        
                await _database.DeleteSessionAsync(_currentSession);
                OnDatabaseChanged?.Invoke();
                GoBack();
            }
        }
    }

    private void OnCancelClicked(object sender, EventArgs e) => GoBack();

    private async void GoBack()
    {
        try
        {
            await Navigation.PopModalAsync();
        } catch {}
    }

    private void ReturnPage(object? sender, EventArgs e)
    {
        GoBack();
    }
}