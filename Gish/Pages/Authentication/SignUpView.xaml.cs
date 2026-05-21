using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Gish.Pages.Classes;
using Gish.Pages.MainPages;

namespace Gish.Pages.Authentication;

public partial class SignUpView : ContentView
{
    private readonly LocalDatabase _database = new();
    private List<Button> cachedButtons = new();
    private List<ImageButton> cachedImgButtons = new();
    
    public SignUpView()
    {
        InitializeComponent();
        this.Loaded += (s, e) => setAllButtonState(true);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        cachedButtons = App.getAllButtons(this);
        cachedImgButtons = App.getAllImageButtons(this);
        setAllButtonState(true);
    }

    private async void SubmitSignUp(object? sender, EventArgs e)
    {
        string username = UsernameInput.Text;
        string email = EmailInput.Text;
        string password = PasswordInput.Text;
        string confirm_pass = ConfirmPassInput.Text;
        
        if (IsEmptyInput(username) || IsEmptyInput(email) ||
            IsEmptyInput(password) || IsEmptyInput(confirm_pass))
        {
            ShowError("Please input all fields");
            return;
        }

        if (!IsValidEmail(email))
        {
            ShowError("Invalid email address");
            return;
        }

        if (!IsValidPassword(password)) return;

        if (!string.Equals(password, confirm_pass))
        {
            ShowError("Passwords do not match");
            return;
        }
        
        RemoveError();
        LoadingUIState(true);

        bool success = await SignupUser(username, email, password);

        if (success)
        {
            GoToMain();
        }
        else
        {
            LoadingUIState(false);
        }
    }

    public async Task<bool> SignupUser(string username, string email, string password)
    {
        try
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new UserAccount
            {
                Username = username,
                EmailAddress = email,
                PasswordHashed = hashedPassword
            };

            int result = await _database.SaveUserAsync(newUser);

            if (result <= 0)
            {
                ShowError("Unknown error has occurred");
                return false;
            }

            int? userID = await _database.getUserID(email);

            if (userID is null)
            {
                ShowError("Unknown error has occurred");
                return false;
            }
            
            await App.setUserID(userID.Value);
            return true;
        }
        catch (SQLite.SQLiteException e) when (e.Message.Contains("constraint", StringComparison.OrdinalIgnoreCase))
        {
            ShowError("Email address already exists");
            return false;
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
            return false;
        }
    }

    private void GoToMain()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                App.SetMainPage(new Gish.Pages.MainPages.MainContainerPage());
            }
            catch (Exception ex)
            {
                ShowError($"Navigation error: {ex.Message}");
                LoadingUIState(false);
            }
        });
    }
    
    private bool IsEmptyInput(string input) => string.IsNullOrWhiteSpace(input);

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email,
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
            RegexOptions.IgnoreCase);
    }

    private bool IsValidPassword(string password)
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

    private void ShowError(string errorMsg)
    {
        InputError.Text = errorMsg;
        InputError.IsVisible = true;
    }
    
    private void RemoveError() => InputError.IsVisible = false;
    
    private void LoadingUIState(bool isLoading)
    {
        SubmitBtn.IsEnabled = !isLoading;
        LoadingIndicator.IsRunning = isLoading;
    }
    
    private void InputChanged(object? sender, TextChangedEventArgs e) => RemoveError();
    
    private void ReturnPage(object? sender, EventArgs e)
    {
        Element current = this.Parent;
        while (current != null)
        {
            if (current is AuthContainerPage container)
            {
                container.SwitchToAuthView("Startup");
                return;
            }
            current = current.Parent;
        }
    }

    private void ToggleShowPassword(object? sender, EventArgs e) => UpdatePasswordState(PasswordInput, TogglePasswordBtn);
    private void ToggleShowConfirmPassword(object? sender, EventArgs e) => UpdatePasswordState(ConfirmPassInput, ToggleConfirmPasswordBtn);
    
    private void UpdatePasswordState(Entry passwordInput, ImageButton toggleButton)
    {
        string openEye = "show_pass_eye.png";
        string closedEye = "show_pass_close_eye.png";
        passwordInput.IsPassword = !passwordInput.IsPassword;
        toggleButton.Source = (passwordInput.IsPassword) ? openEye : closedEye;
    }

    private void setAllButtonState(bool enable)
    {
        App.setButtonState(cachedButtons, enable);
        App.setImageButtonState(cachedImgButtons, enable);
    }
}