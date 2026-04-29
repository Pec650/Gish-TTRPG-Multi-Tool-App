namespace Gish.Pages.MainPages;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnSampleButtonClicked(object sender, EventArgs e)
    {
        await DisplayAlertAsync("Home Page", "Sample backend action executed.", "OK");
    }
}