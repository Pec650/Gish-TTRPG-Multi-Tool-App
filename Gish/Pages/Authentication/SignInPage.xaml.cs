namespace Gish.Pages.Authentication;

using System.Threading.Tasks;

public partial class SignInPage
{
    public SignInPage()
    {
        InitializeComponent();
    }
    
    protected async override void OnAppearing()
    {
        base.OnAppearing();

        if (App.isLoggedIn())
        {
            await GoToMain();
            return;
        }

        PasswordInput.IsPassword = true;
        UpdatePasswordState();
        ResetUIStates();
    }

    private async void SubmitLogin(object? sender, EventArgs e)
    {
        if (IsEmptyInput(EmailInput.Text) || IsEmptyInput(PasswordInput.Text))
        {
            ShowError("Please enter a username and password");
            return;
        }
        
        RemoveError();
        LoadingUIState(true);

        bool success = true;

        if (success)
        {
            await GoToMain();
            return;
        }
        else
        {
            LoadingUIState(false);
        }
    }

    private async Task GoToMain()
    {
        await Shell.Current.GoToAsync("//MainPages/HomeTab/HomePage");
    }

    private bool IsEmptyInput(String input)
    {
        return String.IsNullOrWhiteSpace(input);
    }

    private void ShowError(String errorMsg)
    {
        InputError.Text = errorMsg;
        InputError.IsVisible = true;
    }

    private void RemoveError()
    {
        InputError.IsVisible = false;
    }

    private void ToggleShowPassword(object? sender, EventArgs eventArgs)
    {
        PasswordInput.IsPassword = !PasswordInput.IsPassword;
        UpdatePasswordState();
    }

    private void UpdatePasswordState()
    {
        String openEye = "show_pass_eye.png";
        String closedEye = "show_pass_close_eye.png";
        
        TogglePasswordBtn.Source = (PasswordInput.IsPassword) ? openEye : closedEye;
    }

    private void ResetUIStates()
    {
        EmailInput.Text = string.Empty;
        PasswordInput.Text = string.Empty;
        
        RemoveError();
        LoadingUIState(false);
    }

    private void LoadingUIState(bool isLoading)
    {
        if (isLoading)
        {
            SubmitBtn.IsEnabled = false;
            LoadingIndicator.IsRunning = true;
        }
        else
        {
            SubmitBtn.IsEnabled = true;
            LoadingIndicator.IsRunning = false;
        }
    }

    private void InputChanged(object? sender, TextChangedEventArgs e)
    {
        RemoveError();
    }

    private void ReturnPage(object? sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}