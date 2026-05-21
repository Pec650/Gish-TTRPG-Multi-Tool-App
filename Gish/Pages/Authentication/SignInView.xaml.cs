using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Gish.Pages.Classes;
using Gish.Pages.MainPages;

namespace Gish.Pages.Authentication;

public partial class SignInView : ContentView
{
    private readonly LocalDatabase _database = new();
    
    public SignInView()
    {
        InitializeComponent();
        
        this.Loaded += (s, e) => {
            PasswordInput.IsPassword = true;
            UpdatePasswordState();
            ResetUIStates();
        };
    }

    private async void SubmitLogin(object? sender, EventArgs e)
    {
        string email = EmailInput.Text;
        string password = PasswordInput.Text;
        
        if (IsEmptyInput(email) || IsEmptyInput(password))
        {
            ShowError("Please enter a username and password");
            return;
        }
        
        RemoveError();
        LoadingUIState(true);

        bool success = await SigninUser(email, password);

        if (success)
        {
            GoToMain();
        }
        else
        {
            LoadingUIState(false);
        }
    }
    
    public async Task<bool> SigninUser(string email, string password)
    {
        try
        {
            bool userFound = await _database.matchUserByEmailPassword(email, password);

            if (!userFound)
            {
                ShowError("Incorrect email or password");
                return false;
            }
            
            int? userID = await _database.getUserID(email);

            if (userID is null)
            {
                ShowError("Unknown error has occurred");
                return false;
            }
        
            App.setUserID(userID.Value);
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
        // Force the execution back onto the device UI loop thread
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

    private void ShowError(string errorMsg)
    {
        InputError.Text = errorMsg;
        InputError.IsVisible = true;
    }

    private void RemoveError() => InputError.IsVisible = false;

    private void ToggleShowPassword(object? sender, EventArgs eventArgs)
    {
        PasswordInput.IsPassword = !PasswordInput.IsPassword;
        UpdatePasswordState();
    }

    private void UpdatePasswordState()
    {
        string openEye = "show_pass_eye.png";
        string closedEye = "show_pass_close_eye.png";
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
}