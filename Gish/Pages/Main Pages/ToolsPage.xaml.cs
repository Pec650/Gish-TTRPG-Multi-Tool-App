using SQLite;
using Gish.Pages.Classes;
using Gish.Pages.Main_Pages.Tools_Pages;

namespace Gish.Pages.MainPages;

public partial class ToolsPage : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();
    
    public ToolsPage()
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

    private async void GoToInitiativeTracker(object? sender, EventArgs e)
    {
        try
        {
            setAllButtonState(false);
            await Navigation.PushModalAsync(new Initiative_Tracker());
        }
        catch
        {
            setAllButtonState(true);
        }
    }
}