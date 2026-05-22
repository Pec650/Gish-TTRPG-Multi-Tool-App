using System;
using System.IO;
using Microsoft.Maui.Controls;
using Gish.Pages.Classes;
using Gish.Pages.MainPages;

namespace Gish.Pages.Controls;

public partial class CustomHeader : ContentView
{
    private readonly LocalDatabase _database = new();

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(CustomHeader),
            "Welcome",
            propertyChanged: OnTitleChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public CustomHeader()
    {
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        LoadProfileImage();
    }

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomHeader header)
        {
            header.HeaderTitleLabel.Text = (string)newValue;
        }
    }

    private async void LoadProfileImage()
    {
        try
        {
            UserAccount user = await _database.getUserInfo(App.getUserID());
            if (user?.ProfileImage is not null)
            {
                ProfileBtn.Source = ImageSource.FromStream(() => new MemoryStream(user.ProfileImage));
            }
        }
        catch
        {
            // Fallback gracefully if database context fails
        }
    }

    private void OnProfileClicked(object sender, EventArgs e)
    {
        if (Application.Current?.MainPage is MainContainerPage mainContainer)
        {
            mainContainer.SwitchToTab("Profile");
        }
    }
}