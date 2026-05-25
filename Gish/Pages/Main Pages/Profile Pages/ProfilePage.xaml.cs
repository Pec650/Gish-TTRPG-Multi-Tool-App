using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Gish.Pages.Classes;
using Gish.Pages.Main_Pages.Profile_Pages;

namespace Gish.Pages.MainPages.Profile_Pages;

public partial class ProfilePage : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();
    
    public ProfilePage()
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

        cachedButtons = App.GetAllButtons(this);
        cachedImgButtons = App.GetAllImageButtons(this);

        setAllButtonState(true);
    }
    
    private void setAllButtonState(bool enable)
    {
        App.SetButtonState(cachedButtons, enable);
        App.SetImageButtonState(cachedImgButtons, enable);
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
        setAllButtonState(false);
        try
        {
            await Navigation.PopModalAsync();
        }
        catch
        {
            setAllButtonState(true);
        }
        
    }
    
    private async void GoToEditProfile(object? sender, EventArgs e)
    {
        setAllButtonState(false);
        try
        {
            await Navigation.PushModalAsync(new EditProfilePage());
        }
        catch
        {
            setAllButtonState(true);
        }
    }
    
    private async void LogOut(object? sender, EventArgs e)
    {
        int tempID = App.GetUserId();
        setAllButtonState(false);
        try
        {
            App.ResetUserId();
            await Shell.Current.GoToAsync("//StartupPage");
        }
        catch
        {
            App.SetUserId(tempID);
            setAllButtonState(true);
        }
    }

    private async void GoToChangePasswordButton(object? sender, EventArgs e)
    {
        setAllButtonState(false);
        try
        {
            await Navigation.PushModalAsync(new ChangePasswordPage());
        }
        catch
        {
            setAllButtonState(true);
        }
    }
}