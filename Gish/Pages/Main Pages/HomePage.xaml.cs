using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using SQLite;
using Gish.Pages.Classes;

namespace Gish.Pages.MainPages;

public partial class HomePage : ContentPage
{
    private string _modifier = "N";
    private int _bonus = 0;
    private readonly Random _rng = new();
    private readonly ObservableCollection<string> _rollLog = new();
    private bool _rollLogVisible = false;
    private bool _hasRolledThisSession = false;

    // Tracks how many of each die type are queued
    private readonly Dictionary<int, int> _diceQueue = new()
    {
        { 4, 0 }, { 6, 0 }, { 8, 0 }, { 10, 0 }, { 12, 0 }, { 100, 0 }
    };
    
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();

    public class HomebrewPreview
    {
        public string Name    { get; set; } = "";
        public string SubType { get; set; } = "";
        public string System  { get; set; } = "";
    }

    public HomePage()
    {
        InitializeComponent();
        LoadMockData();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        setAllButtonState(true);
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        SetUserInfo();
        
        cachedButtons = App.getAllButtons(this);
        cachedImgButtons = App.getAllImageButtons(this);

        setAllButtonState(true);
    }
    
    public async void SetUserInfo()
    {
        try
        {
            UserAccount user = await _database.getUserInfo(App.getUserID());

            if (user is not null)
            {

                if (user.ProfileImage is not null)
                {
                    ProfileBtn.Source = ImageSource.FromStream(() => new MemoryStream(user.ProfileImage));
                }
            }
        }
        catch
        {}
    }

    private void LoadMockData()
    {
        RecentHomebrewList.ItemsSource = new List<HomebrewPreview>
        {
            new() { Name = "Eldritch Knight",  SubType = "Class: Fighter", System = "D&D 5.5e" },
            new() { Name = "Khoravare",         SubType = "Type: Humanoid", System = "D&D 5.5e" },
            new() { Name = "Copper Dragonborn", SubType = "CR: 3",          System = "D&D 5.5e" },
        };
        RollLogList.ItemsSource = _rollLog;
    }

    private async void OnHomebrewSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is HomebrewPreview item)
        {
            await DisplayAlertAsync("Homebrew", $"Tapped: {item.Name}", "OK");
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private void OnDiceClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton btn && int.TryParse(btn.CommandParameter?.ToString(), out int sides))
        {
            // D20 button pressed
            if (sides == 20)
            {
                // Condition: If queue is empty, add 1d20. 
                // If queue is NOT empty, we don't add d20, just proceed to roll what's there.
                bool isQueueEmpty = !_diceQueue.Any(kv => kv.Value > 0);
                
                if (isQueueEmpty)
                {
                    // Temporarily add a d20 to the queue for this roll logic
                    // (Note: Since we clear the queue after rolling, this is safe)
                    _diceQueue[20] = 1; 
                }

                OnRollClicked(sender, e);
                return;
            }

            // Other dice (d4, d6, etc.) — add 1 to queue up to 50
            if (_diceQueue.ContainsKey(sides))
            {
                if (_diceQueue[sides] < 50)
                    _diceQueue[sides]++;
                UpdateDiceQueueLabel();
            }
        }
    }

    private void OnModifierClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
            _modifier = btn.CommandParameter?.ToString() ?? "N";
    }

    private void OnBonusUp(object sender, EventArgs e)
    {
        _bonus++;
        BonusLabel.Text = _bonus.ToString();
    }

    private void OnBonusDown(object sender, EventArgs e)
    {
        _bonus--;
        BonusLabel.Text = _bonus.ToString();
    }

    private void OnClearDiceClicked(object sender, EventArgs e)
    {
        foreach (var key in _diceQueue.Keys.ToList())
            _diceQueue[key] = 0;
        UpdateDiceQueueLabel();
    }

    private void UpdateDiceQueueLabel()
    {
        var active = _diceQueue
            .Where(kv => kv.Value > 0)
            .Select(kv => $"{kv.Value}d{kv.Key}");

        if (active.Any())
        {
            DiceQueueLabel.Text = string.Join("  ", active);
        }
        else
        {
            // If they've already rolled once, don't show the help text anymore
            DiceQueueLabel.Text = _hasRolledThisSession 
                ? "" 
                : "Tap dice to add, press d20 to roll";
        }
    }

    private void OnRollLogToggled(object sender, EventArgs e)
    {
        _rollLogVisible = !_rollLogVisible;
        RollLogList.IsVisible = _rollLogVisible;
    }

    private void OnRollClicked(object sender, EventArgs e)
    {
        var results = new List<string>();
        int total = 0;

        // 1. Process all dice currently in the queue (including the d20 if it was added)
        // We sort by key descending so d20/d100 usually show up first in the string
        foreach (var sides in _diceQueue.Keys.OrderByDescending(k => k).ToList())
        {
            int count = _diceQueue[sides];
            if (count <= 0) continue;

            var rolls = new List<int>();
            for (int i = 0; i < count; i++)
            {
                int roll;
                // Handle Advantage/Disadvantage ONLY for d20s
                if (sides == 20 && _modifier != "N")
                {
                    int r1 = _rng.Next(1, 21);
                    int r2 = _rng.Next(1, 21);
                    roll = (_modifier == "A") ? Math.Max(r1, r2) : Math.Min(r1, r2);
                    results.Add($"d20 ({(_modifier == "A" ? "Adv" : "Dis")}): {r1} vs {r2} → {roll}");
                }
                else
                {
                    roll = _rng.Next(1, sides + 1);
                    rolls.Add(roll);
                }
                total += roll;
            }

            if (rolls.Any())
            {
                results.Add($"{count}d{sides}: [{string.Join(", ", rolls)}]");
            }
        }

        // 2. Add the Bonus
        total += _bonus;
        if (_bonus != 0)
            results.Add($"Bonus: {(_bonus > 0 ? "+" : "")}{_bonus}");

        // 3. Update UI and Log
        string logEntry = string.Join(" | ", results) + $" = {total}";
        RollResultLabel.Text = $"Result: {total}";
        _rollLog.Insert(0, logEntry);

        // 4. CLEAR the dice queue and update label
        _hasRolledThisSession = true;

        foreach (var key in _diceQueue.Keys.ToList())
        {
            _diceQueue[key] = 0;
        }
        
        UpdateDiceQueueLabel();
    }
    
    private async void goToProfilePage(object? sender, EventArgs e)
    {
        try
        {
            setAllButtonState(false);
            await Shell.Current.GoToAsync("//ProfilePage");
        }
        catch
        {
            setAllButtonState(true);
        }
    }
    
    private void setAllButtonState(bool enable)
    {
        App.setButtonState(cachedButtons, enable);
        App.setImageButtonState(cachedImgButtons, enable);
    }
}