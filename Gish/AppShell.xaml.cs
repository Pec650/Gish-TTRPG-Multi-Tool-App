namespace ToDoList;

using ToDoList.Pages;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(NewTask), typeof(NewTask));
        Routing.RegisterRoute(nameof(EditTask), typeof(EditTask));
    }
}