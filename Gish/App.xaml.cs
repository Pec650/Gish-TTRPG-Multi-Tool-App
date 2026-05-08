namespace Gish;

using Gish.Pages.Classes;

public partial class App : Application
{
    private static int currentUserID = -1;
    
    public App()
    {
        InitializeComponent();
    }

    protected override async void OnStart()
    {
        await initUserID();
    }
    
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
    
    private async Task initUserID()
    {
        string idString = await SecureStorage.Default.GetAsync("Current_User_ID");

        if (int.TryParse(idString, out int userId))
        {
            currentUserID = userId;
        }

        currentUserID = -1;
    }

    public static async void setUserID(int userID)
    {
        currentUserID = userID;
        await SecureStorage.Default.SetAsync("Current_User_ID", userID.ToString());
    }

    public static int getUserID()
    {
        return currentUserID;
    }

    public static void resetUserID()
    {
        currentUserID = -1;
        SecureStorage.Default.Remove("Current_User_ID");
    }

    public static bool isLoggedIn()
    {
        return currentUserID >= 0;
    }
    
    public static List<Button> getAllButtons(Element parent)
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
            
                buttons.AddRange(getAllButtons((Element)child));
            }
        }
        return buttons;
    }
    
    public static void setButtonState(List<Button> buttons, bool enable)
    {
        buttons.ForEach(btn => btn.IsEnabled = enable);
    }
    
    public static List<ImageButton> getAllImageButtons(Element parent)
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
            
                buttons.AddRange(getAllImageButtons((Element)child));
            }
        }
        return buttons;
    }
    
    public static void setImageButtonState(List<ImageButton> buttons, bool enable)
    {
        buttons.ForEach(btn => btn.IsEnabled = enable);
    }
}