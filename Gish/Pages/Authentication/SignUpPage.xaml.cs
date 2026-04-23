namespace ToDoList.Pages;

using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class SignUpPage
{
    public SignUpPage()
    {
        InitializeComponent();
    }

    private async void SubmitSignUp(object? sender, EventArgs e)
    {
        if (IsEmptyInput(UsernameInput.Text) || IsEmptyInput(EmailInput.Text) ||
            IsEmptyInput(PasswordInput.Text) || IsEmptyInput(ConfirmPassInput.Text))
        {
            ShowError("Please input all fields");
            return;
        }

        if (!IsValidEmail(EmailInput.Text))
        {
            ShowError("Invalid email address");
            return;
        }

        if (!IsValidPassword(PasswordInput.Text))
        {
            return;
        }

        if (!String.Equals(PasswordInput.Text, ConfirmPassInput.Text))
        {
            ShowError("Passwords do not match");
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
        
        LoadingUIState(false);
    }

    private async Task GoToMain()
    {
        await Shell.Current.GoToAsync("//MainPages/HomeTab/HomePage");
    }
    
    private bool IsEmptyInput(String input)
    {
        return String.IsNullOrWhiteSpace(input);
    }

    private bool IsValidEmail(String email)
    {
        return Regex.IsMatch(email,
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
            RegexOptions.IgnoreCase);
    }

    private bool IsValidPassword(String password)
    {
        if (password.Length < 8 || password.Length > 16)
        {
            ShowError("Password must be 8-16 letters");
            return false;
        }

        if (!password.Any(char.IsLetterOrDigit))
        {
            ShowError("Password must have a digit [0-9]");
            return false;
        }

        if (!password.Any(char.IsLower))
        {
            ShowError("Password must have a lowercase [a-z]");
            return false;
        }
        
        if (!password.Any(char.IsUpper))
        {
            ShowError("Password must have a uppercase [A-Z]");
            return false;
        }

        return true;
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

    private void ToggleShowPassword(object? sender, EventArgs e)
    {
        UpdatePasswordState(PasswordInput, TogglePasswordBtn);
    }

    private void ToggleShowConfirmPassword(object? sender, EventArgs e)
    {
        UpdatePasswordState(ConfirmPassInput, ToggleConfirmPasswordBtn);
    }
    
    private void UpdatePasswordState(Entry passwordInput, ImageButton toggleButton)
    {
        String openEye = "show_pass_eye.png";
        String closedEye = "show_pass_close_eye.png";
        
        passwordInput.IsPassword = !passwordInput.IsPassword;
        toggleButton.Source = (passwordInput.IsPassword) ? openEye : closedEye;
    }
}