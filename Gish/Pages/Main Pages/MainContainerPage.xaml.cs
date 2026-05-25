namespace Gish.Pages.Main_Pages;

public partial class MainContainerPage
{
    private readonly View _homeView;
    private readonly View _creationsView;
    private readonly View _toolsView;
    private readonly View _rulesView;

    private readonly List<string> _tabHistory = new();
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

 public void SwitchToTab(string tabName)
    {
        ContentArea.Children.Clear();

        if (!_isInternalTabSwitch)
        {
            if (tabName == "Home")
            {
                _tabHistory.Clear();
                _tabHistory.Add("Home");
            }
            else
            {
                _tabHistory.Remove(tabName);
                _tabHistory.Add(tabName);

                if (_tabHistory.Count > 4)
                {
                    _tabHistory.RemoveAt(1);
                }
            }
        }

        AppTabBar.ActiveTab = tabName;
        AppHeader.Title = tabName;

        switch (tabName)
        {
            case "Home":
                ContentArea.Children.Add(_homeView);
                break;
            case "Creations":
                ContentArea.Children.Add(_creationsView);
                break;
            case "Tools":
                ContentArea.Children.Add(_toolsView);
                break;
            case "Rules":
                ContentArea.Children.Add(_rulesView);
                break;
        }
    }

    public void SetTabBarVisibility(bool isVisible)
    {
        AppTabBar.IsVisible = isVisible;
        ContentArea.Padding = isVisible ? new Thickness(0, 0, 0, 64) : new Thickness(0);
    }
}