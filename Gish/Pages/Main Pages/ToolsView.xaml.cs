using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace Gish.Pages.MainPages;

public partial class ToolsView : ContentView
{
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
        
        cachedButtons = App.getAllButtons(this);
        cachedImgButtons = App.getAllImageButtons(this);
        setAllButtonState(true);
    }

    private void setAllButtonState(bool enable)
    {
        App.setButtonState(cachedButtons, enable);
        App.setImageButtonState(cachedImgButtons, enable);
    }
}