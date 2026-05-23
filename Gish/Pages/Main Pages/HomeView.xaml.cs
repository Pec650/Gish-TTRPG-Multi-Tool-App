using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Gish.Pages.MainPages;

public partial class HomeView : ContentView
{
    private string _modifier = "N";
    private int _bonus = 0;
    private readonly Random _rng = new();
    private readonly ObservableCollection<string> _rollLog = new();
    private readonly Dictionary<int, int> _diceQueue = new()
    {
        [4] = 0,
        [6] = 0,
        [8] = 0,
        [10] = 0,
        [12] = 0,
        [20] = 0,
        [100] = 0,
    };
    private bool _hasRolledThisSession = false;
    private bool _rollLogVisible = true;
    
    private List<Button> cachedButtons = new();
    private List<ImageButton> cachedImgButtons = new();

    public class HomebrewPreview
    {
        public string Name    { get; set; } = "";
        public string SubType { get; set; } = "";
        public string System  { get; set; } = "";
    }

    public HomeView()
    {
        InitializeComponent();
        LoadMockData();

        this.Loaded += (s, e) => setAllButtonState(true);
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        cachedButtons = App.getAllButtons(this);
        cachedImgButtons = App.getAllImageButtons(this);

        setAllButtonState(true);
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
        UpdateDiceQueueLabel();
    }

    private async void OnHomebrewSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is HomebrewPreview item)
        {
            if (Application.Current?.MainPage is Page mainPage)
            {
                await mainPage.DisplayAlertAsync("Homebrew", $"Tapped: {item.Name}", "OK");
            }
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private void OnDiceClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton btn && int.TryParse(btn.CommandParameter?.ToString(), out int sides))
        {
            if (sides == 20)
            {
                bool isQueueEmpty = !_diceQueue.Any(kv => kv.Value > 0);

                if (isQueueEmpty)
                {
                    _diceQueue[20] = 1;
                }

                OnRollClicked(sender, e);
                return;
            }

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

    private void OnRollClicked(object sender, EventArgs e)
    {
        var results = new List<string>();
        int total = 0;

        foreach (var sides in _diceQueue.Keys.OrderByDescending(k => k).ToList())
        {
            int count = _diceQueue[sides];
            if (count <= 0) continue;

            var rolls = new List<int>();
            for (int i = 0; i < count; i++)
            {
                int roll;
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
                RollResult.IsVisible = true;
            }
        }

        total += _bonus;
        if (_bonus != 0)
            results.Add($"Bonus: {(_bonus > 0 ? "+" : "")}{_bonus}");

        string logEntry = string.Join(" | ", results) + $" = {total}";
        RollResultLabel.Text = $"Result: {total}";
        _rollLog.Insert(0, logEntry);

        _hasRolledThisSession = true;
        foreach (var key in _diceQueue.Keys.ToList())
        {
            _diceQueue[key] = 0;
        }
        UpdateDiceQueueLabel();
    }
    
    private void OnClearClicked(object sender, EventArgs e)
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
    
    private void setAllButtonState(bool enable)
    {
        App.setButtonState(cachedButtons, enable);
        App.setImageButtonState(cachedImgButtons, enable);
    }
}