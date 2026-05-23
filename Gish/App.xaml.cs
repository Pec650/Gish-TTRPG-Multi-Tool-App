using Gish.Pages.Authentication;
using Gish.Pages.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Gish;

public partial class App : Application
{
    private static int currentUserID = -1;
    
    public App()
    {
        InitializeComponent();

        if (!isLoggedIn())
        {
            MainPage = new Gish.Pages.Authentication.AuthContainerPage();
        }
        else
        {
            MainPage = new Gish.Pages.MainPages.MainContainerPage();
        }
        
    }
    
    // Direct view swapper that guarantees zero purple platform title bars
    public static void SetMainPage(Page page)
    {
        if (Application.Current is not null)
        {
            Application.Current.MainPage = page;
        }
    }

    protected override async void OnStart()
    {
        try
        {
            await initUserID();
        }
        catch
        {
            // Ignore secure storage initialization failures at startup.
        }
    }
    
    private async Task initUserID()
    {
        string idString = await SecureStorage.Default.GetAsync("Current_User_ID");

        if (int.TryParse(idString, out int userId))
        {
            currentUserID = userId;
        }
        else
        {
            currentUserID = -1;
        }
    }

    public static async Task setUserID(int userID)
    {
        currentUserID = userID;
        try
        {
            await SecureStorage.Default.SetAsync("Current_User_ID", userID.ToString());
        }
        catch
        {
            // If secure storage fails, keep the in-memory ID so login can continue.
        }
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
    
    // VISUAL TREE HELPERS (Used by Startup page to anti-spam button clicks)
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