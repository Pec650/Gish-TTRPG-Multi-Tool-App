using Gish.Pages.Classes;
using Gish.Pages.Authentication;

namespace Gish.Pages.Main_Pages.Profile_Pages;

public partial class ProfilePage
{
    private readonly LocalDatabase _database = new();

    private List<Button> _cachedButtons = [];
    private List<ImageButton> _cachedImgButtons = [];
    
    public ProfilePage()
    {
        InitializeComponent();
        
        SetUserInfo();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        SetAllButtonState(true);
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        SetUserInfo();

        _cachedButtons = App.GetAllButtons(this);
        _cachedImgButtons = App.GetAllImageButtons(this);

        SetAllButtonState(true);
    }
    
    private void SetAllButtonState(bool enable)
    {
        App.SetButtonState(_cachedButtons, enable);
        App.SetImageButtonState(_cachedImgButtons, enable);
    }
    
    public async void SetUserInfo()
    {
        try
        {
            UserAccount user = await _database.getUserInfo(App.GetUserId());

            if (user is not null)
            {
                UsernameText.Text = user.Username;
                EmailText.Text = user.EmailAddress;

                if (user.ProfileImage is not null)
                {
                    ProfilePictureImage.Source = ImageSource.FromStream(() => new MemoryStream(user.ProfileImage));
                }
            }
            else
            {
                UsernameText.Text = "Error";
                EmailText.Text = "Error";
            }
        }
        catch
        {
            UsernameText.Text = "Error";
            EmailText.Text = "Error";
        }
    }

    private async void ReturnPage(object? sender, EventArgs e)
    {
        try
        {
            SetAllButtonState(false);
            await Navigation.PopModalAsync();
        }
        catch
        {
            SetAllButtonState(true);
        }
        
    }
    
    private async void GoToEditProfile(object? sender, EventArgs e)
    {
        try
        {
            SetAllButtonState(false);
            await Navigation.PushModalAsync(new EditProfilePage());
        }
        catch
        {
            SetAllButtonState(true);
        }
    }
    
    private void LogOut(object? sender, EventArgs e)
    {
        SetAllButtonState(false);
        App.ResetUserId();
        App.SetMainPage(new NavigationPage(new Startup()));
    }

    private async void GoToChangePasswordButton(object? sender, EventArgs e)
    {
        try
        {
            SetAllButtonState(false);
            await Navigation.PushModalAsync(new ChangePasswordPage());
        }
        catch
        {
            SetAllButtonState(true);
        }
    }
}