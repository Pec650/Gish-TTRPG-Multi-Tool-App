using Gish.Pages.Classes;

namespace Gish.Pages.Main_Pages;

public partial class MainContainerPage
{
    private readonly View _homeView;
    private readonly View _creationsView;
    private readonly View _toolsView;
    private readonly View _rulesView;

    private List<string> _tabHistory = new();
    private bool _isInternalTabSwitch = false;

    public MainContainerPage()
    {
        InitializeComponent();

        _homeView = new HomeView(); 
        _creationsView = new CreationsView();
        _toolsView = new ToolsView();
        _rulesView = new RulesView();

        _tabHistory.Add("Home");
        SwitchToTab("Home");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        AppHeader.OnAppearing();

        SignalAppearing();
    }
    
    public void SignalAppearing()
    {
        AppTabBar.IsEnabled = true; 

        if (ContentArea.Children.Count > 0)
        {
            var activeView = ContentArea.Children[0];

            if (activeView is IControlToggleable toggleablePage)
            {
                toggleablePage.IsAppearing();
            }
        }
    }

    public void SwitchToTab(string tabName)
    {
        ContentArea.Children.Clear();

        AppTabBar.ActiveTab = tabName;
        AppHeader.Title = tabName;

        View? viewToDisplay = tabName switch
        {
            "Home" => _homeView,
            "Creations" => _creationsView,
            "Tools" => _toolsView,
            "Rules" => _rulesView,
            _ => null
        };

        if (viewToDisplay != null)
        {
            viewToDisplay.HorizontalOptions = LayoutOptions.Fill;
            viewToDisplay.VerticalOptions = LayoutOptions.Fill;
            
            ContentArea.Children.Add(viewToDisplay);
            
            SignalAppearing();
        }
    }
}