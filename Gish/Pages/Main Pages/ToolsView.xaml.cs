using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Maui.Controls;
using Gish.Pages.Classes;
using Gish.Pages.MainPages.Profile_Pages;

namespace Gish.Pages.MainPages;

public partial class ToolsView : ContentView
{
    private readonly LocalDatabase _database = new();
    private List<Button> cachedButtons = new();
    private List<ImageButton> cachedImgButtons = new();
    
    public ToolsView()
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
    
    public async void SetUserInfo()
    {
        try
        {
            UserAccount user = await _database.getUserInfo(App.getUserID());
            if (user?.ProfileImage is not null)
            {
                ProfileBtn.Source = ImageSource.FromStream(() => new MemoryStream(user.ProfileImage));
            }
        }
        catch { }
    }

    private void goToProfilePage(object? sender, EventArgs e)
    {
        try
        {
            setAllButtonState(false);
            
            // Pass "Profile" token string to your MainContainerPage engine layout view system handler
            if (Application.Current?.MainPage is MainContainerPage mainContainer)
            {
                mainContainer.SwitchToTab("Profile");
            }
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
}