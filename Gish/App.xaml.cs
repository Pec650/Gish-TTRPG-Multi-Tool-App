namespace Gish;

public partial class App
{
    private static int _currentUserId = -1;
    
    public App()
    {
        InitializeComponent();
    }

    protected override async void OnStart()
    {
        try
        {
            await InitUserId();
        }
        catch
        {
            // ignored
        }
    }
    
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new Pages.Authentication.Startup()));
    }
    
    public static void SetMainPage(Page rootPage)
    {
        if (Current?.Windows.Count > 0)
        {
            Current.Windows[0].Page = rootPage;
        }
    }
    
    private static async Task InitUserId()
    {
        var idString = await SecureStorage.Default.GetAsync("Current_User_ID");

        if (int.TryParse(idString, out int userId))
        {
            _currentUserId = userId;
        }
        else
        {
            _currentUserId = -1;
        }
    }

    public static async void SetUserId(int userId)
    {
        try
        {
            _currentUserId = userId;
            await SecureStorage.Default.SetAsync("Current_User_ID", userId.ToString());
        }
        catch
        {
            // ignored
        }
    }

    public static int GetUserId()
    {
        return _currentUserId;
    }

    public static void ResetUserId()
    {
        _currentUserId = -1;
        SecureStorage.Default.Remove("Current_User_ID");
    }

    public static async Task<bool> IsLoggedIn()
    {
        try
        {
            _currentUserId = int.Parse(await SecureStorage.Default.GetAsync("Current_User_ID") ?? "-1");
        }
        catch
        {
            // ignored
        }
        return _currentUserId >= 0;
    }
    
    public static List<Button> GetAllButtons(Element parent)
    {
        List<Button> buttons = new List<Button>();

        if (parent is IVisualTreeElement container)
        {
            foreach (var child in container.GetVisualChildren())
            {
                if (child is Button button)
                {
                    buttons.Add(button);
                }
            
                buttons.AddRange(GetAllButtons((Element)child));
            }
        }
        return buttons;
    }
    
    public static void SetButtonState(List<Button> buttons, bool enable)
    {
        buttons.ForEach(btn => btn.IsEnabled = enable);
    }
    
    public static List<ImageButton> GetAllImageButtons(Element parent)
    {
        List<ImageButton> buttons = new List<ImageButton>();

        if (parent is IVisualTreeElement container)
        {
            foreach (var child in container.GetVisualChildren())
            {
                if (child is ImageButton button)
                {
                    buttons.Add(button);
                }
            
                buttons.AddRange(GetAllImageButtons((Element)child));
            }
        }
        return buttons;
    }
    
    public static void SetImageButtonState(List<ImageButton> buttons, bool enable)
    {
        buttons.ForEach(btn => btn.IsEnabled = enable);
    }
}