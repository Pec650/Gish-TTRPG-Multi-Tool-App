using System.Collections.ObjectModel;
using Gish.Pages.Classes;
using Gish.Pages.Main_Pages.Creations_Pages;
using Gish.Pages.Main_Pages.Tools_Pages;

namespace Gish.Pages.Main_Pages;

public partial class HomeView : ContentView, IControlToggleable
{
    private readonly LocalDatabase _database = new();

    private int _bonus;
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
    private bool _hasRolledThisSession;
    private bool _rollLogVisible = true;
    private bool _isD20DrawerOpen;
    private int _d20PoolCount = 1;
    
    private List<Button> _cachedButtons;
    private List<ImageButton> _cachedImgButtons;

    public string CurrentModifier
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            UpdateModifierUi();
        }
    } = "N";


    public HomeView()
    {
        InitializeComponent();
        
        _cachedButtons = App.GetAllButtons(this);
        _cachedImgButtons = App.GetAllImageButtons(this);

        Loaded += (_, _) => SetAllButtonState(true);
    }
    
    protected override async void OnHandlerChanged()
    {
        try
        {
            base.OnHandlerChanged();
            SetAllButtonState(true);
        
            GameSession? nextSession = await _database.GetNextUpcomingSessionAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                RecentSessionItem.ItemsSource = nextSession != null ? new List<GameSession> { nextSession } : null;
            });
        
            try
            {
                var creations = await _database.GetRecentCreations();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    RecentHomebrewList.ItemsSource = creations.Any() ? new ObservableCollection<Creations>(creations) : new ObservableCollection<Creations>();
                });
            }
            catch
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    RecentHomebrewList.ItemsSource = new ObservableCollection<Creations>();
                });
            }
        }
        catch
        {
            // ignored
        }
    }

    private void OnDiceClicked(object sender, EventArgs e)
    {
        if (sender is not ImageButton btn || !int.TryParse(btn.CommandParameter?.ToString(), out var sides)) return;
        
        if (!_diceQueue.ContainsKey(sides) || _diceQueue[sides] >= 50) return;
        
        _diceQueue[sides]++;
        UpdateDiceQueueLabel();
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

    private void UpdateModifierUi()
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

        var enumerable = active as string[] ?? active.ToArray();
        DiceQueueLabel.Text = enumerable.Any()
            ? string.Join("  ", enumerable)
            : (_hasRolledThisSession ? "" : "Tap dice to add, press d20 to roll");
    }

    private void OnRollLogToggled(object sender, EventArgs e)
    {
        _rollLogVisible = !_rollLogVisible;
        RollLogList.ItemsSource = _rollLog;
        RollLogPanel.IsVisible = _rollLogVisible && _rollLog.Count > 0;
        RollLogChevron.Text = _rollLogVisible ? "▲" : "▼";
    }

    public void SetAllButtonState(bool enable)
    {
        App.SetButtonState(_cachedButtons, enable);
        App.SetImageButtonState(_cachedImgButtons, enable);
    }

    private async void OnHomebrewTapped(object? sender, TappedEventArgs e)
    {
        try
        {
            SetAllButtonState(false);

            var layout = sender as BindableObject;
            if (layout == null) {
                SetAllButtonState(true);
                return;
            }

            var selectedCreation = layout.BindingContext as Creations;
            if (selectedCreation == null)
            {
                SetAllButtonState(true);
                return;
            }

            await Navigation.PushModalAsync(new ViewCreationPage(selectedCreation));
        }
        catch
        {
            SetAllButtonState(true);
        }
    }

    private async void GoToNewCreation(object? sender, EventArgs e)
    {
        SetAllButtonState(false);
        try
        {
            await Navigation.PushModalAsync(new NewCreationPage());
            SetAllButtonState(true);
        }
        catch
        {
            SetAllButtonState(true);
        }
    }

    private async void GoToScheduler(object? sender, TappedEventArgs e)
    {
        SetAllButtonState(false);
        try
        {
            await Navigation.PushModalAsync(new SchedulerPage());
            SetAllButtonState(true);
        }
        catch
        {
            SetAllButtonState(true);
        }
    }
}