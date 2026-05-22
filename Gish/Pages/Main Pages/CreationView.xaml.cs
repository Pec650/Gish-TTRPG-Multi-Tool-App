using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Gish.Pages.MainPages;

public partial class CreationsView : ContentView
{
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
        
        cachedButtons = App.getAllButtons(this);
        cachedImgButtons = App.getAllImageButtons(this);
        setAllButtonState(true);
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