namespace Gish.Pages.Authentication;

using System.Threading.Tasks;
using Classes;
using Main_Pages;

public partial class SignInPage
{
    private readonly LocalDatabase _database = new LocalDatabase();

    public SignInPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        PasswordInput.IsPassword = true;
        UpdatePasswordState();
        ResetUiStates();
    }

    [Obsolete("Obsolete")]
    private async void SubmitLogin(object? sender, EventArgs e)
    {
        try
        {
            string email = EmailInput.Text;
            string password = PasswordInput.Text;
        
            if (IsEmptyInput(email) || IsEmptyInput(password))
            {
                ShowError("Please enter a username and password");
                return;
            }
        
            RemoveError();
            LoadingUiState(true);

            bool success = await SigninUser(email, password);

            if (success)
            {
                GoToMain();
            }
            else
            {
                LoadingUiState(false);
            }
        }
        catch (Exception ex)
        {
            ShowError("Unknown error has occurred");
            LoadingUiState(false);
        }
    }
    
    private async Task<bool> SigninUser(string email, string password)
    {
        try
        {
            bool userFound = await _database.matchUserByEmailPassword(email, password);

            if (!userFound)
            {
                ShowError("Incorrect email or password");
                return false;
            }
            
            int? userId = await _database.getUserID(email);

            if (userId is null)
            {
                ShowError("User not found, please try again");
                return false;
            }
        
            App.SetUserId(userId.Value);
            return true;
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
            return false;
        }
    }

    private void GoToMain()
    {
        App.SetMainPage(new MainContainerPage());
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

    private void ResetUiStates()
    {
        EmailInput.Text = string.Empty;
        PasswordInput.Text = string.Empty;
        
        RemoveError();
        LoadingUiState(false);
    }

    private void LoadingUiState(bool isLoading)
    {
        if (isLoading)
        {
            SetAllButtonState(false);
            LoadingIndicator.IsRunning = true;
        }
        else
        {
            SetAllButtonState(true);
            LoadingIndicator.IsRunning = false;
        }
    }
    
    private void SetAllButtonState(bool isEnable)
    {
        BackButtonBtn.IsEnabled = isEnable;
        SubmitBtn.IsEnabled = isEnable;
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