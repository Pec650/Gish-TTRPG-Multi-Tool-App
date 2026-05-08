using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Gish.Pages.Classes;

namespace Gish.Pages.MainPages;

public partial class ProfilePage : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    
    public ProfilePage()
    {
        InitializeComponent();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        App.setButtonState(cachedButtons, true);

        SetUserInfo();
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
            }
            else
            {
                UsernameText.Text = "Error";
                EmailText.Text = "Error";
            }
        }
        catch (Exception ex)
        {
            UsernameText.Text = "Error";
            EmailText.Text = "Error";
        }
    }

    private async void ReturnPage(object? sender, EventArgs e)
    {
        App.setButtonState(cachedButtons, false);
        try
        {
            await Shell.Current.GoToAsync("//MainPages/HomeTab/HomePage");
        }
        catch (Exception ex)
        {
            App.setButtonState(cachedButtons, true);
        }
        
    }
    
    private async void LogOut(object? sender, EventArgs e)
    {
        int tempID = App.getUserID();
        App.setButtonState(cachedButtons, false);
        try
        {
            App.resetUserID();
            await Shell.Current.GoToAsync("//StartupPage");
        }
        catch (Exception ex)
        {
            App.setUserID(tempID);
            App.setButtonState(cachedButtons, true);
        }
    }
}