using System;
using System.Collections.Generic;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Gish.Pages.MainPages;

public partial class RulesView : ContentView
{
    private List<Button> cachedButtons = new();
    private List<ImageButton> cachedImgButtons = new();
    
    public RulesView()
    {
        InitializeComponent();
        
        this.Loaded += (s, e) => setAllButtonState(true);
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        cachedButtons = App.getAllButtons(this);
        cachedImgButtons = App.getAllImageButtons(this);
        setAllButtonState(true);
    }
    
    private void setAllButtonState(bool enable)
    {
        App.setButtonState(cachedButtons, enable);
        App.setImageButtonState(cachedImgButtons, enable);
    }

    private void PlayersHandbookDirectory(object? sender, TappedEventArgs e)
    {
        goToLink("https://online.anyflip.com/mldog/ynbn/mobile/");
    }

    private void PlayersHandbook2014Directory(object? sender, TappedEventArgs e)
    {
        goToLink("https://online.anyflip.com/sqwmo/hzys/mobile/index.html");
    }

    private void PathfinderPlayersGuideDirectory(object? sender, TappedEventArgs e)
    {
        goToLink("https://online.anyflip.com/njoma/bvqf/mobile/index.html");
    }

    private async void goToLink(string url)
    {
        try
        {
            await Launcher.Default.OpenAsync(url);
        }
        catch
        {
            if (Application.Current?.MainPage is Page mainPage)
            {
                await mainPage.DisplayAlert("Error", "Could not open the link.", "OK");
            }
        }
    }
}