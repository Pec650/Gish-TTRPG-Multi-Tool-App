using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    private async void LogOut(object? sender, EventArgs e)
    {
        App.resetUser();
        await Shell.Current.GoToAsync("//StartupPage");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        FullName.Text = App.CurrentUser.fname +  " " + App.CurrentUser.lname;
    }
}