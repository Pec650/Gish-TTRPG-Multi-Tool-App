using Gish.Pages.Main_Pages.Tools_Pages;

namespace Gish.Pages.Main_Pages;

public partial class ToolsView
{
    private readonly List<Button> _cachedButtons;
    private readonly List<ImageButton> _cachedImgButtons;
    
    public ToolsView()
    {
        InitializeComponent();
        
        _cachedButtons = App.GetAllButtons(this);
        _cachedImgButtons = App.GetAllImageButtons(this);
        SetAllButtonState(true);
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        SetAllButtonState(true);
    }

    private void SetAllButtonState(bool enable)
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
}