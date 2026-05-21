using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Maui.Controls;
using Gish.Pages.Classes;
using Gish.Pages.MainPages.Profile_Pages;

namespace Gish.Pages.MainPages;

public partial class CreationsView : ContentView
{
    private readonly LocalDatabase _database = new();
    private List<Button> cachedButtons = new();
    private List<ImageButton> cachedImgButtons = new();
    
    public CreationsView()
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

    private void UpdateActiveTags(object? sender, TappedEventArgs e)
    {
        if (sender is ContentView contentView)
        {
            var border = contentView.GetVisualTreeDescendants().OfType<Border>().FirstOrDefault();
            var label = contentView.GetVisualTreeDescendants().OfType<Label>().FirstOrDefault();
            var icon = contentView.GetVisualTreeDescendants().OfType<Image>().FirstOrDefault();

            if (border != null && label != null && icon != null)
            {
                string tagType = contentView.StyleId;
                bool isActive = border.BackgroundColor.Equals(Color.FromArgb("#FE5F55"));
                
                if (isActive)
                {
                    border.BackgroundColor = Color.FromArgb("#EEE0CB");
                    label.TextColor = Color.FromArgb("#4F6367");
                }
                else
                {
                    border.BackgroundColor = Color.FromArgb("#FE5F55");
                    label.TextColor = Colors.White;
                }
                
                string prefix = isActive ? "" : "active_";
                switch (tagType)
                {
                    case "Subclass":
                        icon.Source = $"{prefix}subclass_tag_icon.svg";
                        break;
                    case "Lineage":
                        icon.Source = $"{prefix}lineage_tag_icon.svg";
                        break;
                    case "Monster":
                        icon.Source = $"{prefix}monster_tag_icon.svg";
                        break;
                    case "Spell":
                        icon.Source = $"{prefix}spells_tag_icon.svg";
                        break;
                    case "Feat":
                        icon.Source = $"{prefix}feat_tag_icon.svg";
                        break;
                }
            }
        }
    }
}