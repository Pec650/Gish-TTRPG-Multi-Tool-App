namespace Gish.Pages.MainPages;

public partial class CreationsPage : ContentPage
{
    public CreationsPage()
    {
        InitializeComponent();
    }

    private async void goToProfilePage(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ProfilePage");
    }
}