namespace Gish.Pages.Controls;

public partial class CustomTabBar : ContentView
{
    public static readonly BindableProperty ActiveTabProperty =
        BindableProperty.Create(nameof(ActiveTab), typeof(string), typeof(CustomTabBar),
            defaultValue: "Home", propertyChanged: OnActiveTabChanged);

    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    private static void OnActiveTabChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CustomTabBar bar)
            bar.UpdateActiveTab((string)newValue);
    }

    public CustomTabBar()
    {
        InitializeComponent();
        Loaded += (s, e) => UpdateActiveTab(ActiveTab);
    }

    private void UpdateActiveTab(string activeTab)
    {
        Color active   = Color.FromArgb("#424C44");
        Color inactive = Color.FromArgb("#424C44");
        float activeOpacity   = 1.0f;
        float inactiveOpacity = 0.45f;

        HomeIcon.Opacity      = activeTab == "Home"      ? activeOpacity : inactiveOpacity;
        CreationsIcon.Opacity = activeTab == "Creations" ? activeOpacity : inactiveOpacity;
        ToolsIcon.Opacity     = activeTab == "Tools"     ? activeOpacity : inactiveOpacity;
        RulesIcon.Opacity     = activeTab == "Rules"     ? activeOpacity : inactiveOpacity;

        HomeLabel.FontAttributes      = activeTab == "Home"      ? FontAttributes.Bold : FontAttributes.None;
        CreationsLabel.FontAttributes = activeTab == "Creations" ? FontAttributes.Bold : FontAttributes.None;
        ToolsLabel.FontAttributes     = activeTab == "Tools"     ? FontAttributes.Bold : FontAttributes.None;
        RulesLabel.FontAttributes     = activeTab == "Rules"     ? FontAttributes.Bold : FontAttributes.None;
    }

    private async void OnHomeTapped(object? sender, TappedEventArgs e)
    {
        if (ActiveTab == "Home") return;
        await Shell.Current.GoToAsync("//HomePage");
    }

    private async void OnCreationsTapped(object? sender, TappedEventArgs e)
    {
        if (ActiveTab == "Creations") return;
        await Shell.Current.GoToAsync("//CreationsPage");
    }

    private async void OnToolsTapped(object? sender, TappedEventArgs e)
    {
        if (ActiveTab == "Tools") return;
        await Shell.Current.GoToAsync("//ToolsPage");
    }

    private async void OnRulesTapped(object? sender, TappedEventArgs e)
    {
        if (ActiveTab == "Rules") return;
        await Shell.Current.GoToAsync("//RulesPage");
    }
}