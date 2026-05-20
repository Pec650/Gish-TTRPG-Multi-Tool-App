using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Gish.Pages.Classes;

namespace Gish.Pages.MainPages.Profile_Pages;

public partial class ChangePasswordPage : ContentPage
{
    private LocalDatabase _database = new LocalDatabase();
    
    private List<Button> cachedButtons = new List<Button>();
    private List<ImageButton> cachedImgButtons = new List<ImageButton>();
    
    public ChangePasswordPage()
    {
        InitializeComponent();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        setAllButtonState(true);
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        cachedButtons = App.getAllButtons(this);
        cachedImgButtons = App.getAllImageButtons(this);

        setAllButtonState(true);
    }
    
    private void setAllButtonState(bool enable)
    {
        App.setButtonState(cachedButtons, enable);
        App.setImageButtonState(cachedImgButtons, enable);
    }
    
    private async void ReturnPage(object? sender, EventArgs e)
    {
        setAllButtonState(false);
        try
        {
            await Navigation.PopModalAsync();
        }
        catch
        {
            setAllButtonState(true);
        }
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

    private void InputChanged(object? sender, TextChangedEventArgs e)
    {
        RemoveError();
    }

    private async void ConfirmChange(object? sender, EventArgs e)
    {
        string password = PasswordInput.Text;
        string confirm_pass = ConfirmPassInput.Text;
    
        if (IsEmptyInput(password) || IsEmptyInput(confirm_pass))
        {
            ShowError("Please input all fields");
            return;
        }

        if (!IsValidPassword(password))
        {
            return;
        }

        if (!String.Equals(password, confirm_pass))
        {
            ShowError("Passwords do not match");
            return;
        }
    
        RemoveError();
        LoadingUIState(true);

        bool success = await UpdatePassword(password);
        
        if (success)
        {
            await GoToProfilePage();
        }
    
        LoadingUIState(false);
    }

    private async Task<bool> UpdatePassword(String password)
    {
        try
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            
            UserAccount user = await _database.getUserInfo(App.getUserID());

            if (user is null)
            {
                ShowError("Unknown error has occurred");
                return false;
            }

            user.PasswordHashed = hashedPassword;
            
            bool updateSuccess = await _database.updateUserInfo(user);

            if (!updateSuccess)
            {
                ShowError("Unknown error has occurred");
                return false;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
            return false;
        }
    }
    
    private async Task GoToProfilePage()
    {
        try
        {
            await Navigation.PopModalAsync();
        }
        catch
        {
            ShowError("Unable to navigate to profile page");
        }
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
    
    private bool IsEmptyInput(String input)
    {
        return String.IsNullOrWhiteSpace(input);
    }
    
    private void LoadingUIState(bool isLoading)
    {
        if (isLoading)
        {
            setAllButtonState(false);
            LoadingIndicator.IsRunning = true;
        }
        else
        {
            setAllButtonState(true);
            LoadingIndicator.IsRunning = false;
        }
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
}