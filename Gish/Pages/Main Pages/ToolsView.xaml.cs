using Gish.Pages.Main_Pages.Tools_Pages;
using Gish.Pages.Classes;
using Gish.Pages.ToolPages;

namespace Gish.Pages.Main_Pages;

public partial class ToolsView : ContentView, IControlToggleable
{
    private List<Button> _cachedButtons;
    private List<ImageButton> _cachedImgButtons;
    
    public ToolsView()
    {
        InitializeComponent();
        
        _cachedButtons = App.GetAllButtons(this);
        _cachedImgButtons = App.GetAllImageButtons(this);
        SetAllButtonState(true);
    }
    
    public void IsAppearing()
    {
        SetAllButtonState(true);
    }

    public void SetAllButtonState(bool enable)
    {
        App.SetButtonState(_cachedButtons, enable);
        App.SetImageButtonState(_cachedImgButtons, enable);
    }

    private void GoToInitiativeTracker(object? sender, EventArgs e)
    {
        SetAllButtonState(false);
        Navigation.PushModalAsync(new Initiative_Tracker());
    }

    private void GoToScheduler(object? sender, EventArgs e)
    {
        SetAllButtonState(false);
        Navigation.PushModalAsync(new SchedulerPage());
    }

    private void GoToCRCalculator(object? sender, EventArgs e)
    {
        SetAllButtonState(false);
        Navigation.PushModalAsync(new CRCalculatorPage());
    }
}