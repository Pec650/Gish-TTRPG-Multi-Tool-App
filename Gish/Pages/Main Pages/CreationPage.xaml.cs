using SQLite;
using Gish.Pages.Classes;
using Gish.Pages.Main_Pages.Creations_Pages;
using Gish.Pages.MainPages.Profile_Pages;

namespace Gish.Pages.MainPages;

public partial class CreationsPage : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();

    private string searchString = "";
    private bool hasSubclass = true;
    private bool hasLineage = true;
    private bool hasMonster = true;
    private bool hasSpell = true;
    private bool hasFeat = true;
    
    public CreationsPage()
    {
        InitializeComponent();
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        setAllButtonState(true);

        try
        {
            CreationsListView.ItemsSource = await _database.GetAllCreations(searchString, hasSubclass, hasLineage, hasMonster, hasSpell, hasFeat);
        } catch {}
        
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

    private async void goToProfilePage(object? sender, EventArgs e)
    {
        try
        {
            setAllButtonState(false);
            await Navigation.PushModalAsync(new ProfilePage());
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

    private async void UpdateActiveTags(object? sender, TappedEventArgs e)
    {
        if (sender is ContentView)
        {
            var contentView = (ContentView)sender;

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
                        hasSubclass = !isActive;
                        break;
                    case "Lineage":
                        icon.Source = ((isActive) ? "" : "active_") + "lineage_tag_icon.svg";
                        hasLineage = !isActive;
                        break;
                    case "Monster":
                        icon.Source = ((isActive) ? "" : "active_") + "monster_tag_icon.svg";
                        hasMonster = !isActive;
                        break;
                    case "Spell":
                        icon.Source = ((isActive) ? "" : "active_") + "spells_tag_icon.svg";
                        hasSpell = !isActive;
                        break;
                    case "Feat":
                        icon.Source = ((isActive) ? "" : "active_") + "feat_tag_icon.svg";
                        hasFeat = !isActive;
                        break;
                }
                
                try
                {
                    CreationsListView.ItemsSource = await _database.GetAllCreations(searchString, hasSubclass, hasLineage, hasMonster, hasSpell, hasFeat);
                } catch {}
            }
        }
    }

    private async void GoToNewCreation(object? sender, TappedEventArgs e)
    {
        try
        {
            setAllButtonState(false);
            await Navigation.PushModalAsync(new NewCreationPage());
        }
        catch
        {
            setAllButtonState(true);
        }
    }

    private async void OnCreationTapped(object? sender, TappedEventArgs e)
    {
        try
        {
            setAllButtonState(false);

            var layout = sender as BindableObject;
            if (layout == null) {
                setAllButtonState(true);
                return;
            }

            var selectedCreation = layout.BindingContext as Creations;
            if (selectedCreation == null)
            {
                setAllButtonState(true);
                return;
            }

            await Navigation.PushModalAsync(new ViewCreationPage(selectedCreation));
        }
        catch
        {
            setAllButtonState(true);
        }
        
    }

    private async void SearchInputChange(object? sender, TextChangedEventArgs e)
    {
        searchString = SearchInput.Text;
        
        try
        {
            CreationsListView.ItemsSource = await _database.GetAllCreations(searchString, hasSubclass, hasLineage, hasMonster, hasSpell, hasFeat);
        } catch {}
    }

    private async void OnClickGoToNewCreation(object? sender, EventArgs e)
    {
        try
        {
            setAllButtonState(false);
            await Navigation.PushModalAsync(new NewCreationPage());
        }
        catch
        {
            setAllButtonState(true);
        }
    }
}