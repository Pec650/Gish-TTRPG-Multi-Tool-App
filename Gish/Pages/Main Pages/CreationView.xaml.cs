using Gish.Pages.Classes;
using Gish.Pages.Main_Pages.Creations_Pages;

namespace Gish.Pages.Main_Pages;

public partial class CreationsView : ContentView, IControlToggleable
{
    private readonly LocalDatabase _database = new();
    
    private List<Button> _cachedButtons;
    private List<ImageButton> _cachedImgButtons;

    private string _searchString = "";
    private bool _hasSubclass = true;
    private bool _hasLineage = true;
    private bool _hasMonster = true;
    private bool _hasSpell = true;
    private bool _hasFeat = true;
    
    private bool _areInteractionsEnabled = true;

    public bool AreInteractionsEnabled
    {
        get => _areInteractionsEnabled;
        set
        {
            _areInteractionsEnabled = value;
            OnPropertyChanged(nameof(AreInteractionsEnabled));
        }
    }
    
    public CreationsView()
    {
        InitializeComponent();
        
        _cachedButtons = App.GetAllButtons(this);
        _cachedImgButtons = App.GetAllImageButtons(this);
        SetAllButtonState(true);

        LoadCreations();
    }
    
    protected override async void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        SetAllButtonState(true);

        LoadCreations();
    }

    private async void LoadCreations()
    {
        try
        {
            CreationsListView.ItemsSource = await _database.GetAllCreations(_searchString, _hasSubclass, _hasLineage, _hasMonster, _hasSpell, _hasFeat);
        }
        catch
        {
            // ignored
        }
    }

    public void IsAppearing()
    {
        SetAllButtonState(true);
        LoadCreations();
    }
    
    private void SetAllButtonState(bool enable)
    {
        AreInteractionsEnabled = enable;
        App.SetButtonState(_cachedButtons, enable);
        App.SetImageButtonState(_cachedImgButtons, enable);
        FloatingAddBtn.IsEnabled = enable;
    }

    private async void UpdateActiveTags(object? sender, TappedEventArgs e)
    {
        if (sender is ContentView contentView)
        {
            var border = contentView.GetVisualTreeDescendants().OfType<Border>().FirstOrDefault();
            var label = contentView.GetVisualTreeDescendants().OfType<Label>().FirstOrDefault();
            var icon = contentView.GetVisualTreeDescendants().OfType<Image>().FirstOrDefault();

            if (border != null && label != null && icon != null)
            {
                string tagType = contentView.StyleId;

                bool isActive = border.BackgroundColor.Equals(Color.FromArgb("#FE5F55"));
                
                if (isActive)
                {
                    border.BackgroundColor = Color.FromArgb("#EEE0CB");
                    label.TextColor = Color.FromArgb("#4F6367");
                }
                else
                {
                    border.BackgroundColor = Color.FromArgb("#FE5F55");
                    label.TextColor = Colors.White;
                }
                
                switch (tagType)
                {
                    case "Subclass":
                        icon.Source = ((isActive) ? "" : "active_") + "subclass_tag_icon.svg";
                        _hasSubclass = !isActive;
                        break;
                    case "Lineage":
                        icon.Source = ((isActive) ? "" : "active_") + "lineage_tag_icon.svg";
                        _hasLineage = !isActive;
                        break;
                    case "Monster":
                        icon.Source = ((isActive) ? "" : "active_") + "monster_tag_icon.svg";
                        _hasMonster = !isActive;
                        break;
                    case "Spell":
                        icon.Source = ((isActive) ? "" : "active_") + "spells_tag_icon.svg";
                        _hasSpell = !isActive;
                        break;
                    case "Feat":
                        icon.Source = ((isActive) ? "" : "active_") + "feat_tag_icon.svg";
                        _hasFeat = !isActive;
                        break;
                }

                try
                {
                    CreationsListView.ItemsSource = await _database.GetAllCreations(_searchString, _hasSubclass, _hasLineage, _hasMonster, _hasSpell, _hasFeat);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }

    private async void GoToNewCreation(object? sender, TappedEventArgs e)
    {
        try
        {
            SetAllButtonState(false);
            await Navigation.PushModalAsync(new NewCreationPage());
        }
        catch
        {
            SetAllButtonState(true);
        }
    }

    private async void OnCreationTapped(object? sender, TappedEventArgs e)
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

    private async void SearchInputChange(object? sender, TextChangedEventArgs e)
    {
        _searchString = SearchInput.Text;
        
        try
        {
            CreationsListView.ItemsSource = await _database.GetAllCreations(_searchString, _hasSubclass, _hasLineage, _hasMonster, _hasSpell, _hasFeat);
        }
        catch
        {
            // ignored
        }
    }

    private async void OnClickGoToNewCreation(object? sender, EventArgs e)
    {
        try
        {
            SetAllButtonState(false);
            await Navigation.PushModalAsync(new NewCreationPage());
        }
        catch
        {
            SetAllButtonState(true);
        }
    }
}