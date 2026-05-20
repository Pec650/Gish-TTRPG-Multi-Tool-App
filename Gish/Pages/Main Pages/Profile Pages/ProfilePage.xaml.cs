using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Gish.Pages.Classes;
using Gish.Pages.MainPages;
using Gish.Pages.MainPages.Profile_Pages;
using Gish.Pages.Authentication;

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

        cachedButtons = App.getAllButtons(this);
        cachedImgButtons = App.getAllImageButtons(this);

        setAllButtonState(true);
    }
    
    private void setAllButtonState(bool enable)
    {
        App.setButtonState(cachedButtons, enable);
        App.setImageButtonState(cachedImgButtons, enable);
    }
    
    public async void SetUserInfo()
    {
        try
        {
            UserAccount user = await _database.getUserInfo(App.getUserID());

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
            await Navigation.PopAsync();
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
        int tempID = App.getUserID();
        setAllButtonState(false);
        try
        {
            App.resetUserID();
            var nav = Navigation;
            if (nav.NavigationStack.Count > 0)
            {
                nav.InsertPageBefore(new Startup(), nav.NavigationStack[0]);
                await nav.PopToRootAsync(false);
            }
            else
            {
                await nav.PushAsync(new Startup());
            }
        }
        catch
        {
            App.setUserID(tempID);
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