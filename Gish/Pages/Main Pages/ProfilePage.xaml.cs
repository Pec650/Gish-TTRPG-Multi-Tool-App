using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gish.Pages.MainPages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    private async void LogOut(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//StartupPage");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        ReturnButton.IsEnabled = true;
    }

    private async void ReturnPage(object? sender, EventArgs e)
    {
        ReturnButton.IsEnabled = false;
        await Shell.Current.GoToAsync("//MainPages/HomeTab/HomePage");
    }
}