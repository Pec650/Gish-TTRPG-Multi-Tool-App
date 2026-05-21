using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Maui.Controls;
using Gish.Pages.Classes;
using Gish.Pages.Authentication;

namespace Gish.Pages.MainPages.Profile_Pages;

public partial class ProfileView : ContentView
{
    private readonly LocalDatabase _database = new();
    private List<Button> cachedButtons = new();
    private List<ImageButton> cachedImgButtons = new();
    
    public ProfileView()
    {
        InitializeComponent();
        this.Loaded += (s, e) => setAllButtonState(true);
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

    private void ReturnPage(object? sender, EventArgs e)
    {
        setAllButtonState(false);
        try
        {
            // Direct parent view manager call to restore navigation back to your tab engine shell
            if (Application.Current?.MainPage is MainPages.MainContainerPage container)
            {
                container.SwitchToTab("Home");
            }
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
            // Note: Modal views must be pushed using the root window page context!
            if (Application.Current?.MainPage is Page mainPage)
            {
                await mainPage.Navigation.PushModalAsync(new EditProfilePage());
            }
        }
        catch
        {
            setAllButtonState(true);
        }
    }
    
    private async void GoToChangePasswordButton(object? sender, EventArgs e)
    {
        setAllButtonState(false);
        try
        {
            if (Application.Current?.MainPage is Page mainPage)
            {
                await mainPage.Navigation.PushModalAsync(new ChangePasswordPage());
            }
        }
        catch
        {
            setAllButtonState(true);
        }
    }
    
    private void LogOut(object? sender, EventArgs e)
    {
        setAllButtonState(false);
        try
        {
            App.resetUserID();
            
            // Cleanly re-instantiate your complete authentication container view shell window
            App.SetMainPage(new AuthContainerPage());
        }
        catch
        {
            setAllButtonState(true);
        }
    }
}