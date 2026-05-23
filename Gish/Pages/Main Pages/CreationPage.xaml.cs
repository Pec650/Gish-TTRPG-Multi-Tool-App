using SQLite;
using Gish.Pages.Classes;

namespace Gish.Pages.MainPages;

public partial class CreationsPage : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();
    
    public CreationsPage()
    {
        InitializeComponent();
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

    private void UpdateActiveTags(object? sender, TappedEventArgs e)
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
                        break;
                    case "Lineage":
                        icon.Source = ((isActive) ? "" : "active_") + "lineage_tag_icon.svg";
                        break;
                    case "Monster":
                        icon.Source = ((isActive) ? "" : "active_") + "monster_tag_icon.svg";
                        break;
                    case "Spell":
                        icon.Source = ((isActive) ? "" : "active_") + "spells_tag_icon.svg";
                        break;
                    case "Feat":
                        icon.Source = ((isActive) ? "" : "active_") + "feat_tag_icon.svg";
                        break;
                }
            }
        }
    }
}