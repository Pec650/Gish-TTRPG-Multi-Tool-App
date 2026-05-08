namespace Gish.Pages.MainPages;

public partial class RulesPage : ContentPage
{
    public RulesPage()
    {
        InitializeComponent();
    }
    
    private async void goToProfilePage(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ProfilePage");
    }
}