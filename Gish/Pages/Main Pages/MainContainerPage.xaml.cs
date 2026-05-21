using Microsoft.Maui.Controls;

namespace Gish.Pages.MainPages;

public partial class MainContainerPage : ContentPage
{
    // Keep single cached instances of your dashboard sub-views in memory
    private readonly View _homeView;
    private readonly View _creationsView;
    private readonly View _toolsView;
    private readonly View _rulesView;

    public MainContainerPage()
    {
        InitializeComponent();

        // 1. Instantiate the content structures (We will convert these next)
        _homeView = new HomeView(); 
        _creationsView = new CreationsView();
        _toolsView = new ToolsView();
        _rulesView = new RulesView();

        // 2. Load Home by default
        SwitchToTab("Home");
    }

    public void SwitchToTab(string tabName)
    {
        // Clear whatever sub-view is currently displayed
        ContentArea.Children.Clear();

        // Inject the target sub-view into the active frame instantly with NO flash
        switch (tabName)
        {
            case "Home":
                ContentArea.Children.Add(_homeView);
                AppTabBar.ActiveTab = "Home";
                break;
            case "Creations":
                ContentArea.Children.Add(_creationsView);
                AppTabBar.ActiveTab = "Creations";
                break;
            case "Tools":
                ContentArea.Children.Add(_toolsView);
                AppTabBar.ActiveTab = "Tools";
                break;
            case "Rules":
                ContentArea.Children.Add(_rulesView);
                AppTabBar.ActiveTab = "Rules";
                break;
        }
    }
}