using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

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
    private bool _rollLogVisible = false;
    private bool _isD20DrawerOpen = false;
    private int _d20PoolCount = 1;

    private List<Button> cachedButtons = new();
    private List<ImageButton> cachedImgButtons = new();

    public class HomebrewPreview
    {
        public string Name    { get; set; } = "";
        public string SubType { get; set; } = "";
        public string System  { get; set; } = "";
    }

    public string CurrentModifier
    {
        get => _modifier;
        set
        {
            _modifier = value;
            OnPropertyChanged(nameof(CurrentModifier));
            UpdateModifierUI();
        }
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
        UpdateModifierUI();
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
            if (_diceQueue.ContainsKey(sides) && _diceQueue[sides] < 50)
            {
                _diceQueue[sides]++;
                UpdateDiceQueueLabel();
            }
        }
    }

    // --- D20 DRAWER ---

    private void OnToggleD20Drawer(object sender, EventArgs e)
    {
        _isD20DrawerOpen = !_isD20DrawerOpen;
        DiceGridView.IsVisible = !_isD20DrawerOpen;
        D20DrawerView.IsVisible = _isD20DrawerOpen;
        D20DrawerChevron.Text = _isD20DrawerOpen ? "▴" : "▾";
    }

    private void OnIncrementD20Pool(object sender, EventArgs e)
    {
        if (_d20PoolCount < 10)
        {
            _d20PoolCount++;
            D20CountDisplay.Text = $"{_d20PoolCount}d20";
        }
    }

    private void OnDecrementD20Pool(object sender, EventArgs e)
    {
        if (_d20PoolCount > 0)
        {
            _d20PoolCount--;
            D20CountDisplay.Text = $"{_d20PoolCount}d20";
        }
    }

    private void OnExecuteD20DrawerRoll(object sender, EventArgs e)
    {

        _diceQueue[20] = _d20PoolCount;
        ExecuteDiceRoll();

        CloseD20Drawer();
    }

    private void CloseD20Drawer()
    {
        _isD20DrawerOpen = false;
        DiceGridView.IsVisible = true;
        D20DrawerView.IsVisible = false;
        D20DrawerChevron.Text = "▾";
        _d20PoolCount = 1;
        D20CountDisplay.Text = "1d20";
    }

    // --- MODIFIER ---

    private void OnSelectDisadvantage(object sender, EventArgs e) => CurrentModifier = "D";
    private void OnSelectNormal(object sender, EventArgs e) => CurrentModifier = "N";
    private void OnSelectAdvantage(object sender, EventArgs e) => CurrentModifier = "A";

    private void UpdateModifierUI()
    {
        DisadvBlock.BackgroundColor = CurrentModifier == "D" ? Color.FromArgb("#C0392B") : Colors.Transparent;
        NormalBlock.BackgroundColor = CurrentModifier == "N" ? Color.FromArgb("#4A148C") : Colors.Transparent;
        AdvBlock.BackgroundColor    = CurrentModifier == "A" ? Color.FromArgb("#2E7D32") : Colors.Transparent;

        DisadvLabel.TextColor = CurrentModifier == "D" ? Colors.White : Color.FromArgb("#AAB7B8");
        NormalLabel.TextColor = CurrentModifier == "N" ? Colors.White : Color.FromArgb("#AAB7B8");
        AdvLabel.TextColor    = CurrentModifier == "A" ? Colors.White : Color.FromArgb("#AAB7B8");
    }

    // --- BONUS ---

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

    // --- ROLL ENGINE ---

    private void ExecuteDiceRoll()
    {
        var results = new List<string>();
        var visualDiceRows = new List<string>();
        int diceSum = 0;

        for (int i = RollResultContainer.Children.Count - 1; i > 0; i--)
            RollResultContainer.Children.RemoveAt(i);

        foreach (var sides in _diceQueue.Keys.OrderByDescending(k => k).ToList())
        {
            int count = _diceQueue[sides];
            if (count <= 0) continue;

            var rolls = new List<int>();

            if (sides == 20 && CurrentModifier != "N")
            {
                for (int i = 0; i < count; i++)
                {
                    int r1 = _rng.Next(1, 21);
                    int r2 = _rng.Next(1, 21);
                    int chosenRoll = (CurrentModifier == "A") ? Math.Max(r1, r2) : Math.Min(r1, r2);
                    diceSum += chosenRoll;
                    rolls.Add(chosenRoll);

                    string stateLabel = CurrentModifier == "A" ? "(Advantage)" : "(Disadvantage)";
                    visualDiceRows.Add(stateLabel);
                    visualDiceRows.Add($"Result: {chosenRoll}");
                    visualDiceRows.Add($"d20: {r1} vs {r2}");
                    results.Add($"1d20 ({CurrentModifier}): {r1} vs {r2} → {chosenRoll}");
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    int roll = _rng.Next(1, sides + 1);
                    rolls.Add(roll);
                    diceSum += roll;
                }
                if (rolls.Any())
                {
                    visualDiceRows.Add($"({count})d{sides}: [{string.Join(", ", rolls)}]");
                    results.Add($"{count}d{sides}: [{string.Join(", ", rolls)}]");
                }
            }
        }

        int finalTotal = diceSum + _bonus;

        if (_bonus > 0)
            RollResultMainLabel.Text = $"Result: {diceSum} + {_bonus} = {finalTotal}";
        else if (_bonus < 0)
            RollResultMainLabel.Text = $"Result: {diceSum} - {Math.Abs(_bonus)} = {finalTotal}";
        else
            RollResultMainLabel.Text = $"Result: {diceSum} = {finalTotal}";

        foreach (var rowText in visualDiceRows)
        {
            bool isHeader = rowText.StartsWith("Result:") || rowText.StartsWith("(Adv") || rowText.StartsWith("(Dis");
            RollResultContainer.Children.Add(new Label
            {
                Text = rowText,
                FontSize = isHeader ? 13 : 11,
                FontAttributes = isHeader ? FontAttributes.Bold : FontAttributes.None,
                TextColor = isHeader ? Color.FromArgb("#111111") : Color.FromArgb("#556677"),
                HorizontalOptions = LayoutOptions.Center,
                FontFamily = "Monospace"
            });
        }

        if (_bonus != 0)
            results.Add($"Bonus: {(_bonus > 0 ? "+" : "")}{_bonus}");

        string logEntry = string.Join(" | ", results) + $" = {finalTotal}";
        _rollLog.Insert(0, logEntry);

        RollResult.IsVisible = true;
        if (_rollLogVisible)
            RollLogPanel.IsVisible = true;

        _hasRolledThisSession = true;
        foreach (var key in _diceQueue.Keys.ToList())
            _diceQueue[key] = 0;

        UpdateDiceQueueLabel();
    }

    // --- CLEAR: resets queue AND closes d20 drawer ---
    private void OnClearClicked(object sender, EventArgs e)
    {
        foreach (var key in _diceQueue.Keys.ToList())
            _diceQueue[key] = 0;

        if (_isD20DrawerOpen)
            CloseD20Drawer();

        UpdateDiceQueueLabel();
    }

    private void UpdateDiceQueueLabel()
    {
        var active = _diceQueue
            .Where(kv => kv.Value > 0)
            .Select(kv => $"{kv.Value}d{kv.Key}");

        DiceQueueLabel.Text = active.Any()
            ? string.Join("  ", active)
            : (_hasRolledThisSession ? "" : "Tap dice to add, press d20 to roll");
    }

    private void OnRollLogToggled(object sender, EventArgs e)
    {
        _rollLogVisible = !_rollLogVisible;
        RollLogPanel.IsVisible = _rollLogVisible && _rollLog.Count > 0;
        RollLogChevron.Text = _rollLogVisible ? "▲" : "▼";
    }

    private void setAllButtonState(bool enable)
    {
        App.setButtonState(cachedButtons, enable);
        App.setImageButtonState(cachedImgButtons, enable);
    }
}