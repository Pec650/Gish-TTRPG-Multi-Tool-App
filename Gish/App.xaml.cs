using MauiApp1;
using Microsoft.Extensions.DependencyInjection;

namespace ToDoList;

using ToDoList.Pages;

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

    public static void resetUserID()
    {
        currentUserID = -1;
        SecureStorage.Default.Remove("Current_User_ID");
    }

    public static bool isLoggedIn()
    {
        return currentUserID >= 0;
    }
}