using Gish.Pages.Classes;

namespace Gish.Pages.MainPages;

public partial class RulesPage : ContentPage
{
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();
    
    private LocalDatabase _database = new LocalDatabase();
    
    public RulesPage()
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

    private void PlayersHandbookDirectory(object? sender, TappedEventArgs e)
    {
        string url = "https://online.anyflip.com/mldog/ynbn/mobile/";
        goToLink(url);
    }

    private void PlayersHandbook2014Directory(object? sender, TappedEventArgs e)
    {
        string url = "https://online.anyflip.com/sqwmo/hzys/mobile/index.html";
        goToLink(url);
    }

    private void PathfinderPlayersGuideDirectory(object? sender, TappedEventArgs e)
    {
        string url = "https://online.anyflip.com/njoma/bvqf/mobile/index.html";
        goToLink(url);
    }

    private async void goToLink(String url)
    {
        try
        {
            await Launcher.Default.OpenAsync(url);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Could not open the link.", "OK");
        }
    }
}