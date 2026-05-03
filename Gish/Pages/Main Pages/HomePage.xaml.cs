using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace Gish.Pages.MainPages;

public partial class HomePage : ContentPage
{
    private int _selectedDie = 20;
    private string _modifier = "N";
    private int _bonus = 0;
    private readonly Random _rng = new();
    private readonly ObservableCollection<string> _rollLog = new();

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
            _selectedDie = sides;
        else if (sender is Button textBtn && int.TryParse(textBtn.CommandParameter?.ToString(), out int sides2))
            _selectedDie = sides2;
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
        int roll1 = _rng.Next(1, _selectedDie + 1);
        int final;
        string logEntry;

        if (_modifier == "A")
        {
            int roll2 = _rng.Next(1, _selectedDie + 1);
            final = Math.Max(roll1, roll2) + _bonus;
            logEntry = $"D{_selectedDie} (Adv): {roll1} vs {roll2} → {final}";
        }
        else if (_modifier == "D")
        {
            int roll2 = _rng.Next(1, _selectedDie + 1);
            final = Math.Min(roll1, roll2) + _bonus;
            logEntry = $"D{_selectedDie} (Dis): {roll1} vs {roll2} → {final}";
        }
        else
        {
            final = roll1 + _bonus;
            logEntry = $"D{_selectedDie}: {roll1} + {_bonus} → {final}";
        }

        RollResultLabel.Text = $"Result: {final}";
        _rollLog.Insert(0, logEntry);
    }
}