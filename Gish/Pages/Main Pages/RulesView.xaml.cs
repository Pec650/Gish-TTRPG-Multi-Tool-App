using Gish.Pages.Classes;

namespace Gish.Pages.Main_Pages;

public partial class RulesView : ContentView, IControlToggleable
{
    private List<Button> _cachedButtons;
    private List<ImageButton> _cachedImgButtons;
    
    public RulesView()
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
    
    public void IsAppearing()
    {
        SetAllButtonState(true);
    }
    
    private void SetAllButtonState(bool enable)
    {
        App.SetButtonState(_cachedButtons, enable);
        App.SetImageButtonState(_cachedImgButtons, enable);
    }

    [Obsolete("Obsolete")]
    private void PlayersHandbookDirectory(object? sender, TappedEventArgs e)
    {
        string url = "https://online.anyflip.com/mldog/ynbn/mobile/";
        GoToLink(url);
    }

    [Obsolete("Obsolete")]
    private void PlayersHandbook2014Directory(object? sender, TappedEventArgs e)
    {
        string url = "https://online.anyflip.com/sqwmo/hzys/mobile/index.html";
        GoToLink(url);
    }

    [Obsolete("Obsolete")]
    private void PathfinderPlayersGuideDirectory(object? sender, TappedEventArgs e)
    {
        string url = "https://online.anyflip.com/njoma/bvqf/mobile/index.html";
        GoToLink(url);
    }

    [Obsolete("Obsolete")]
    private async void GoToLink(String url)
    {
        try
        {
            await Launcher.Default.OpenAsync(url);
        }
        catch (Exception ex)
        {
            try
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Could not open the link.", "OK");
                }
            }
            catch
            {
                // ignored
            }

            System.Diagnostics.Debug.WriteLine($"URL link failed: {ex.Message}");
        }
    }
}